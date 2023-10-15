using MongoDB.Bson.Serialization.Attributes;

namespace ToDoBA
{
    public class Todo
    {
        [BsonId]
        public int Id { get; set; } =0;
        [BsonElement]
        public string Name { get; set; } = string.Empty;
        [BsonElement]
        public DateTime createDate { get; set; }
        [BsonElement]
        public DateTime? FinishDate { get; set; }
        [BsonElement]
        public bool status { get; set; }
    }
}
