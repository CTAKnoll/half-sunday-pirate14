using UI.Plants;

namespace UI.Model
{
    public interface BucketConsumable
    {
        public bool IsOverBucket(out Bucket consumer);
    }
}