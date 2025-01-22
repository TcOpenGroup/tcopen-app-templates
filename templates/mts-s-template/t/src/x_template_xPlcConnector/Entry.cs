using System;
using System.Linq;
using TcOpen.Inxton.Data.Json;
using TcOpen.Inxton.RepositoryDataSet;
using Vortex.Adapters.Connector.Tc3.Adapter;
using Vortex.Connector;
using x_template_xPlc;

namespace x_template_xPlcConnector
{
    public class Entry
    {

        


        private static x_template_xPlcTwinController plc =  new x_template_xPlcTwinController(Tc3ConnectorAdapter.Create(851,true));

     

        private static ApplicationSettings _settings = new ApplicationSettings();


        public static x_template_xPlc.x_template_xPlcTwinController Plc => plc;

        private static x_template_xPlc.x_template_xPlcTwinController _plcDesign;
        public static x_template_xPlc.x_template_xPlcTwinController PlcDesign
        {
            get
            {
                if (_plcDesign == null) _plcDesign = new x_template_xPlc.x_template_xPlcTwinController(new ConnectorAdapter(typeof(DummyConnector)));
                return _plcDesign;
            }
        }

        public static ApplicationSettings Settings { get { return _settings; } } 

        /// <summary>
        /// Load specific parameters stored in json file stored in 'x_template_xPlcConnector.Properties.Settings.Default.SettingsLocation'
        /// </summary>
        /// <param name="setId">Name for set</param>
        public static void LoadAppSettings(string setId,bool inDevelopingMode)
        {
            if (!inDevelopingMode)
            {

                RepositoryDataSetHandler<ApplicationSettings> _settings = RepositoryDataSetHandler<ApplicationSettings>.CreateSet(new JsonRepository<EntitySet<ApplicationSettings>>(new JsonRepositorySettings<EntitySet<ApplicationSettings>>(Properties.Settings.Default.SettingsLocation)));//todo tco adresar
                var result = _settings.Repository.Queryable.FirstOrDefault(p => p._EntityId == setId);
 
                var set = new EntitySet<ApplicationSettings>();
                set._Modified = DateTime.Now;
                set._EntityId = setId;

                if (result == null)
                {
                    set._Created = DateTime.Now;

                    _settings.Create(setId, set);
                }


            Entry._settings = _settings.Read(setId).Item;
            }
            plc = Entry._settings.DeployMode == DeployMode.Dummy
                ? new x_template_xPlcTwinController(new ConnectorAdapter(typeof(DummyConnector)))
                : Entry._settings.DeployMode == DeployMode.Local
                    ? new x_template_xPlcTwinController(Tc3ConnectorAdapter.Create(851, Settings.ShowConsoleOutput))
                    : new x_template_xPlcTwinController(Tc3ConnectorAdapter.Create(Entry._settings.PlcAmsId, 851, Settings.ShowConsoleOutput));
        }

        /// <summary>
        /// Recursively iterates through an IVortexObject and its children, applying a specified predicate function.
        /// If the predicate returns true, the object can be processed further (e.g., added to a collection).
        /// Handles cases where child objects are arrays or implement IVortexObject.
        /// </summary>
        /// <param name="sourceObj">The root IVortexObject to process.</param>
        /// <param name="include">
        /// A predicate function to determine whether an object meets a specific condition. 
        /// </param>
        public static void SearchOnlinerHelper(IVortexObject sourceObj, Func<object, bool> include = null)
        {

            foreach (var child in sourceObj.GetChildren())
            {


                var obj = child;

                if (obj != null)
                {

                    if (include != null)
                    {

                        if (include(obj))
                        {
                            ;//resultCollection.Add((IVortexObject)obj);
                        }
                        else
                        {



                            if (obj.GetType().IsArray)
                            {
                                var arrayObj = obj as Array;

                                if (arrayObj != null)
                                    for (int i = 0; i < arrayObj.Length; i++)
                                    {
                                        SearchOnlinerHelper((IVortexObject)arrayObj.GetValue(i), include);
                                    }

                            }

                            if (obj is IVortexObject)
                            {
                                SearchOnlinerHelper((IVortexObject)obj, include);
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Recursively iterates through an IPlain and its children, applying a specified predicate function.
        /// If the predicate returns true, the object can be processed further (e.g., added to a collection).
        /// Handles cases where child objects are arrays or implement IVortexObject.
        /// </summary>
        /// <param name="sourceObj">The root IPlain to process.</param>
        /// <param name="include">
        /// A predicate function to determine whether an object meets a specific condition. 
        /// </param>
        public static void SearchPlainerHelper(IPlain sourceObj, Func<object, bool> include = null)
        {

            foreach (var child in sourceObj.GetType().GetProperties().Where(p => true))
            {
                var obj = child.GetValue(sourceObj);
                var objName = child.Name;
                string path = string.Empty;




                if (obj != null)
                {

                    if (include != null)
                    {
                        if (include(obj))
                        {

                            ;// _collectioOfPlainReqType.Add((IPlain)obj);
                        }

                        if (obj.GetType().IsArray)
                        {
                            var arrayObj = obj as Array;

                            if (arrayObj != null)
                                for (int i = 0; i < arrayObj.Length; i++)
                                {
                                    SearchPlainerHelper((IPlain)arrayObj.GetValue(i), include);
                                }
                        }

                        if (obj is IPlain)
                        {
                            SearchPlainerHelper((IPlain)obj, include);
                        }

                    }
                }
            }
        }

    }

}
