
using System;
using System.Collections.Generic;
using System.Linq;
using TcOpen.Inxton.Data;
using Vortex.Connector;
using x_template_xPlc;

namespace x_template_xReworkInstructor.Instructor
{
    public class TransformationEntityData
    {
        private IRepository<PlainProcessData> repository;
        private List<IPlain> _collectioOfPlainReqType = new List<IPlain>();
        private List<IVortexElement> _collectioOfReqType = new List<IVortexElement>();

        public TransformationEntityData(IRepository<PlainProcessData> repos)
        {
            Repository = repos;
            _collectioOfReqType.Clear();
            _collectioOfPlainReqType.Clear();

        }

        public PlainProcessData Source { get; set; }

        public IRepository<PlainProcessData> Repository { get => repository; set => repository = value; }

        public PlainProcessData GetEntityData(string id)
        {
            return Repository.Queryable.FirstOrDefault<PlainProcessData>(p => p._EntityId == id);
        }

        public IQueryable<PlainProcessData> GetEntities()
        {
            return Repository.Queryable.Where(p => true);
        }


        public Func<object, bool> Exclusion { get; }
        public Func<object, bool> Inclusion { get; }


        public void SearchPlainEntity(string symbol, IPlain sourceObj, Func<object, bool> exclude = null, Func<string, object, bool> include = null)
        {

            foreach (var child in sourceObj.GetType().GetProperties().Where(p => true))
            {
                var obj = child.GetValue(sourceObj);
                var objName = child.Name;
                string path = string.Empty;

                path = $"{symbol}.{objName}";


                if (obj != null)
                {

                    if (include != null)
                    {
                        if (include(symbol, obj))
                        {

                            _collectioOfPlainReqType.Add((IPlain)obj);
                        }

                        if (obj.GetType().IsArray)
                        {
                            var arrayObj = obj as Array;

                            if (arrayObj != null)
                                for (int i = 0; i < arrayObj.Length; i++)
                                {
                                    SearchPlainEntity(path, (IPlain)arrayObj.GetValue(i), exclude, include);
                                }
                        }

                        if (obj is IPlain)
                        {
                            SearchPlainEntity(path, (IPlain)obj, exclude, include);
                        }

                    }
                }
            }
        }

        internal void SearchEntity(IVortexObject sourceObj, Func<object, bool> include = null)
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
                            _collectioOfReqType.Add((IVortexObject)obj);
                        }
                        else
                        {



                            if (obj.GetType().IsArray)
                            {
                                var arrayObj = obj as Array;

                                if (arrayObj != null)
                                    for (int i = 0; i < arrayObj.Length; i++)
                                    {
                                        SearchEntity((IVortexObject)arrayObj.GetValue(i), include);
                                    }

                            }

                            if (obj is IVortexObject)
                            {
                                SearchEntity((IVortexObject)obj, include);
                            }
                        }
                    }
                }
            }
        }



    }

}
