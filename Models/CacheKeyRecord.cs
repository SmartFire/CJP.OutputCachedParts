namespace CJP.OutputCachedParts.Models
{
    public class CacheKeyRecord
    {
        public virtual int Id { get; set; }
        public virtual int ContentId { get; set; }
        public virtual string PartName { get; set; }
        public virtual string CacheKey { get; set; }
    }
}