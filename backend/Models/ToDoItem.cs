namespace backend.Models
{
    using System;
    using Newtonsoft.Json;

    public class ToDoItem
    {
        // json 形式で保存されるので、Id から id に変更すること
        [JsonProperty(PropertyName = "id")]
        public Guid Id { get; set; }

        [JsonProperty(PropertyName = "context")]
        public string Context { get; set; }

        [JsonProperty(PropertyName = "email")]
        public string EMail { get; set; }

        [JsonProperty(PropertyName = "createAt")]
        public DateTime CreateAt { get; set; }
    }
}