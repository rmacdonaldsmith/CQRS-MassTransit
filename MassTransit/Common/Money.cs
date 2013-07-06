namespace Common
{
    public struct Money
    {
        private decimal _amount;
        public decimal Amount
        {
            get { return _amount; }
            set { _amount = value; }
        }

        public Money(decimal amount)
        {
            _amount = amount;
        }

        public override string ToString()
        {
            return "$" + Amount;
        }

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }
            return Amount.Equals(((Money)obj).Amount);
        }

        public static bool operator ==(Money c1, Money c2)
        {
            return c1._amount == c2.Amount;
        }

        public static bool operator !=(Money c1, Money c2)
        {
            return !(c1 == c2);
        }

        public static Money operator +(Money c1, Money c2)
        {
            return new Money(c1._amount + c2._amount);
        }

        public static bool operator <(Money c1, Money c2)
        {
            return c1._amount < c2._amount;
        }

        public static bool operator >(Money c1, Money c2)
        {
            return c1._amount > c2._amount;
        }

        public static Money operator -(Money c1, Money c2)
        {
            return new Money(c1._amount - c2._amount);
        }
        public override int GetHashCode()
        {
            return Amount.GetHashCode();
        }
    }

    public static class Extensions
    {
        public static Money Dollars(this decimal amount)
        {
            return new Money(amount);
        }

        public static Money Dollars(this int amount)
        {
            return new Money(amount);
        }
    }
}
