using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Text;
using System.Linq;

using Vortex.Connector;
using System.Collections.ObjectModel;
using TcOpen.Inxton.Data;
using TcOpen.Inxton.RepositoryDataSet;
using x_template_xPlc;
using TcoInspectors;

namespace x_template_xReworkInstructor.Instructor
{
    public class ReworkInstructorController
        : INotifyPropertyChanged
    {
        
        public ReworkInstructorController(RepositoryDataSetHandler<ReworkInstructionItem> instructionData,
                                   IRepository<PlainProcessData> repository,
                                   ProcessData onlineData)
        {
            OnlineData = onlineData;
            DataHandler = instructionData;
            RewrkSetRepository = repository;
            transferEntityObject = new TransformationEntityData(repository);
            RepositorySource = repository;
            RefreshReworkSet(repository);

        }


        /// <summary>
        /// Gets current instruction set.
        /// </summary>
        public EntitySet<ReworkInstructionItem> CurrentInstructionSet { get; set; } = new EntitySet<ReworkInstructionItem>();
        /// <summary>
        /// Gets production of this 
        /// </summary>
        protected RepositoryDataSetHandler<ReworkInstructionItem> DataHandler { get; }
        public IRepository<PlainProcessData> RewrkSetRepository { get; private set; }

        public string ConfigName { get; set; }

        /// <summary>
        /// Source repository
        /// </summary>
        public IRepository<PlainProcessData> RepositorySource { get; private set; }


        ReworkInstructionItem currentInstruction = new ReworkInstructionItem();

        /// <summary>
        /// Gets current instruction item.
        /// </summary>
        public ReworkInstructionItem CurrentInstruction
        {
            get
            {
                return currentInstruction;
            }
            set
            {
                if (currentInstruction == value)
                {
                    return;
                }

                currentInstruction = value;
                OnPropertyChanged(nameof(CurrentInstruction));
            }
        }


        private ReworkInstructionItem EmptyIntruction = new ReworkInstructionItem();

        public void RefreshReworkSet()
        {
            RefreshReworkSet(RepositorySource);
        }
        private void RefreshReworkSet(IRepository<PlainProcessData> repositorySource)
        {
            var items = repositorySource.Queryable.Select(p => p._EntityId).ToList();

            RecipeCollection = new ObservableCollection<string>(items);
            if (!RecipeCollection.Contains(string.Empty))
            {
                RecipeCollection.Add(string.Empty);
            }
        }

 



        public void FindInsturction(string entityId)
        {
            if (string.IsNullOrEmpty(EntityId))
            {
                RefreshInstruction(null);// provide empty instruction
                return;
            }
            var entity = transferEntityObject.GetEntityData(entityId);

            if (entity.EntityHeader.Results.Result == (short)eOverallResult.Failed)
            {
                collectionOfFailedCheck.Clear();

                if (entity != null)
                {
                    transferEntityObject.SearchPlainEntity("", entity, null, IncludeFailedInspectors);

                    // first failed inspector in process data
                    var key = collectionOfFailedCheck[0].Key;

                    if (key != null)
                    {
                        var result = CurrentInstructionSet.Items.FirstOrDefault(p => p.Key == key);

                        RefreshInstruction(result.Key);

               
                      //  this.SaveDataSet(OnlineData.HumanReadable);

                    }

                }

            }
            else
            {
                //send empty instruction
                RefreshInstruction(null);
            }

        }


        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }


        TransformationEntityData transferEntityObject;
      

        public ProcessData OnlineData { get; private set; }

        public string EntityId { get; private set; }
        public string ActualResult { get; private set; }

        public ObservableCollection<string> RecipeCollection
        {
            get => _recipeCollection; private set
            {
                _recipeCollection = value;
                OnPropertyChanged(nameof(RecipeCollection));
            }
        }


        /// <summary>
        /// When overriden performs update of <see cref="CurrentInstruction"/>.
        /// </summary>
        public void RefreshInstruction(string key =null)
        {
            if (key == null)
            {
                this.CurrentInstruction = EmptyIntruction;
                return;
            }
            var instruction = this.CurrentInstructionSet.Items.Where(p => p.Key == key).FirstOrDefault();

            if (instruction != null)
            {
                this.CurrentInstruction = instruction;
            }
            else
            {
                this.CurrentInstruction = EmptyIntruction;
            }
        }
        /// <summary>
        /// Loads items set from the repository to this controller.
        /// </summary>
        /// <param name="setid">set id.</param>
        public void LoadDataSet(string setid)
        {
            var result = DataHandler.Repository.Queryable.FirstOrDefault(p => p._EntityId == setid);

            if (result == null)
            {
                DataHandler.Create(setid, CurrentInstructionSet);
            }

            CurrentInstructionSet = DataHandler.Read(setid);
        }

        /// <summary>
        /// Saves items set from this controller to the repository.
        /// </summary>
        /// <param name="setId">Instrucion set id.</param>
        public void SaveDataSet(string setId)
        {

            if (!DataHandler.Repository.Queryable.Where(p => p._EntityId == setId).Any())
            {
                DataHandler.Create(setId, CurrentInstructionSet);
            }
            DataHandler.Update(setId, CurrentInstructionSet);


        }

        public void UpdateFromOnlineDataTemplate()
        {
            if (!string.IsNullOrEmpty(OnlineData.HumanReadable))
            {
                collectionOfAllCheck.Clear();

                transferEntityObject.SearchEntity( OnlineData, Include);



                UpdateList(collectionOfAllCheck);
              //  this.SaveDataSet(OnlineData.HumanReadable);


            }
        }



        public void UpdateList(IEnumerable<ReworkInstructionItem> templateInstructions)
        {
            var templateInstructionList = new List<ReworkInstructionItem>();

            foreach (var item in templateInstructions)
            {
                templateInstructionList.Add(item);
            }


            foreach (var item in CurrentInstructionSet.Items)
            {
                item.Status = ReworkInstructionItemStatus.Deleted;
            }

            CurrentInstructionSet.Items = new ObservableCollection<ReworkInstructionItem>(CurrentInstructionSet.Items.Union(templateInstructionList));

            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CurrentInstructionSet)));

            foreach (var item in CurrentInstructionSet.Items)
            {
                if (templateInstructions != null)
                {
                    var result = templateInstructions.Where(p => p.Key == item.Key).FirstOrDefault();
                    if (result != null)
                    {
                        item.Status = ReworkInstructionItemStatus.Active;
                    }
                }
            }

   
        }


        public void UpdateFromTemplate()
        {
            if (this.OnlineData!=null)
            {
                UpdateFromOnlineDataTemplate();
            }
       
        }

        private List<ReworkInstructionItem> collectionOfFailedCheck = new List<ReworkInstructionItem>();
        private List<ReworkInstructionItem> collectionOfAllCheck = new List<ReworkInstructionItem>();
        private ObservableCollection<string> _recipeCollection;



        static string GetTextBetweenData(string inputSource,string marker)
        {


            // Find the position of the first occurrence of "_data"
            int firstIndex = inputSource.IndexOf(marker);

            // If the first "_data" is found
            if (firstIndex != -1)
            {
                // Find the position of the last occurrence of "_data"
                int lastIndex = inputSource.LastIndexOf(marker);

                // Ensure the lastIndex is different from the firstIndex
                if (lastIndex != -1 && lastIndex != firstIndex)
                {
                    // Calculate the start and length for the substring
                    int startIndex = firstIndex + marker.Length;
                    int length = lastIndex - startIndex;

                    // Extract the substring between the first and last "_data"
                    return inputSource.Substring(startIndex, length);
                }
            }


            return inputSource;
        }

        private bool Include( object obj)
        {
            const string marker = "._data";

            switch (obj)
            {
     
                case TcoDigitalInspectorData c:
                    collectionOfAllCheck.Add(new ReworkInstructionItem() { Key = GetTextBetweenData(c.Symbol, marker).TrimStart('.'), KeyDescription =$"{c.HumanReadable.TrimEnd('.').Split('.').Last()}", Status = ReworkInstructionItemStatus.Active });
                    return c is TcoDigitalInspectorData;
                case TcoAnalogueInspectorData c:
                    collectionOfAllCheck.Add(new ReworkInstructionItem() { Key = GetTextBetweenData(c.Symbol, marker).TrimStart('.'), KeyDescription = $"{c.HumanReadable.TrimEnd('.').Split('.').Last()}", Status = ReworkInstructionItemStatus.Active });
                    return c is TcoAnalogueInspectorData;
                case TcoDataInspectorData c:
                    collectionOfAllCheck.Add(new ReworkInstructionItem() { Key = GetTextBetweenData(c.Symbol, marker).TrimStart('.'), KeyDescription = $"{c.HumanReadable.TrimEnd('.').Split('.').Last()}", Status = ReworkInstructionItemStatus.Active });
                    return c is TcoDataInspectorData;

                default:
                    break;
            }

            return false;
        }

        private bool IncludeFailedInspectors(string symbol, object obj)
        {
            var result = false;
            switch (obj)
            {

                case PlainTcoDigitalInspectorData c:
                    result = c.Result ==(short)eInspectorResult.Failed;
                    if (result)
                    {
                        collectionOfFailedCheck.Add(new ReworkInstructionItem() { Key = symbol, Status = ReworkInstructionItemStatus.Active });
                    }
                    return c is PlainTcoDigitalInspectorData && result;

                case PlainTcoAnalogueInspectorData c:
                    result = c.Result == (short)eInspectorResult.Failed;
                    if (result)
                    {
                        collectionOfFailedCheck.Add(new ReworkInstructionItem() { Key = symbol, Status = ReworkInstructionItemStatus.Active });
                    }
                    return c is PlainTcoAnalogueInspectorData && result;

                case PlainTcoDataInspectorData c:
                    result = c.Result == (short)eInspectorResult.Failed;
                    if (result)
                    {
                        collectionOfFailedCheck.Add(new ReworkInstructionItem() { Key = symbol, Status = ReworkInstructionItemStatus.Active });

                    }
                    return c is PlainTcoDataInspectorData && result;

                default:
                    break;
            }

            return false;
        }

    }
}
