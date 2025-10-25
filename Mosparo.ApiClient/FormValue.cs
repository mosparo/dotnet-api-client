using System.Collections;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Mosparo.ApiClient
{
    public interface IFormValue
    {
         object getValue();
    }

    public interface IStringFormValue
    {
        string getValueAsString();
    }

    public interface ILongFormValue
    {
        long getValueAsLong();
    }

    public interface IDecimalFormValue
    {
        decimal getValueAsDecimal();
    }

    public interface IBoolFormValue
    {
        bool getValueAsBool();
    }

    public interface IDictionaryFormValue
    {
        SortedDictionary<string, IFormValue> getValueAsDictionary();
    }

    public interface IArrayListFormValue
    {
        ArrayList getValueAsArrayList();
    }

    public class StringFormValue : IFormValue, IStringFormValue
    {
        [JsonInclude]
        private string value;

        public StringFormValue(string value)
        {
            this.value = value;
        }

        public object getValue()
        { 
            return value; 
        }

        public string getValueAsString()
        {
            return value;
        }
    }

    public class LongFormValue : IFormValue, ILongFormValue
    {
        private long value;

        public LongFormValue(long value)
        {
            this.value = value;
        }

        public object getValue()
        {
            return value;
        }

        public long getValueAsLong()
        {
            return value;
        }
    }

    public class DecimalFormValue : IFormValue, IDecimalFormValue
    {
        private decimal value;

        public DecimalFormValue(decimal value)
        {
            this.value = value;
        }

        public object getValue()
        {
            return value;
        }

        public decimal getValueAsDecimal()
        {
            return value;
        }
    }

    public class BoolFormValue : IFormValue, IBoolFormValue
    {
        private bool value;

        public BoolFormValue(bool value)
        {
            this.value = value;
        }

        public object getValue()
        {
            return value;
        }

        public bool getValueAsBool()
        {
            return value;
        }
    }

    public class NullFormValue : IFormValue
    {
        public object getValue()
        {
            return null;
        }
    }

    public class DictionaryFormValue : IFormValue, IDictionaryFormValue
    {
        private SortedDictionary<string, IFormValue> value;

        public DictionaryFormValue(SortedDictionary<string, IFormValue> value)
        {
            this.value = value;
        }

        public object getValue()
        {
            return value;
        }

        public SortedDictionary<string, IFormValue> getValueAsDictionary()
        {
            return value;
        }
    }

    public class ArrayListFormValue : IFormValue, IArrayListFormValue
    {
        private ArrayList value;

        public ArrayListFormValue(ArrayList value)
        {
            this.value = value;
        }

        public object getValue()
        {
            return value;
        }

        public ArrayList getValueAsArrayList()
        {
            return value;
        }
    }
}
