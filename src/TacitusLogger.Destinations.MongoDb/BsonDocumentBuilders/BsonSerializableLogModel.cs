using System;
using System.Collections.Generic;
using System.Text;

namespace TacitusLogger.Destinations.MongoDb.BsonDocumentBuilders
{
    public class BsonSerializableLogModel
    {
        public string LogId;
        public string Context;
        public string[] Tags;
        public string Source;
        public LogType LogType;
        public string Description;
        public LogItem[] LogItems;
        public DateTime LogDate;

        public BsonSerializableLogModel()
        {

        }
        public BsonSerializableLogModel(LogModel logModel)
        {
            if (logModel == null)
                return;

            LogId = logModel.LogId;
            Context = logModel.Context;
            Tags = logModel.Tags;
            Source = logModel.Source;
            LogType = logModel.LogType;
            Description = logModel.Description;
            LogItems = logModel.LogItems;
            LogDate = logModel.LogDate;
        }
    }
}
