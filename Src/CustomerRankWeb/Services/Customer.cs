namespace CustomerRankWeb.Services
{
    public class Customer : IComparable<Customer>, IEqualityComparer<Customer>, ICloneable
    {
        public long Id { get; set; }
        public decimal Score { get; set; }

        public object Clone()
        {
            return this.MemberwiseClone();
        }

        public int CompareTo(Customer? other)
        {
            if (other == null)
            {
                return -1;
            }
            if (other.Score == Score)
            {
                return Id.CompareTo(other.Id);
            }
            else
            {
                return Score.CompareTo(other.Score) * -1;
            }
        }

        public bool Equals(Customer x, Customer y)
        {
            return x.Id == y.Id;
        }

        public int GetHashCode(Customer obj)
        {
            return Id.GetHashCode();
        }
    }
}
