using System.Collections;
using Xunit;

namespace Mosparo.ApiClient.Tests
{
    public class StringFormValueTests
    {
        [Fact()]
        public void getValueTest()
        {
            string str = "testvalue1";
            StringFormValue obj = new StringFormValue(str);

            Xunit.Assert.Equal(str, obj.getValue() as string);
            Xunit.Assert.Equal(str, obj.getValueAsString());
        }
    }
    
    public class LongFormValueTests
    {
        [Fact()]
        public void getValueTest()
        {
            long number = 123;
            LongFormValue obj = new LongFormValue(number);

            Xunit.Assert.Equal(number, (long) obj.getValue());
            Xunit.Assert.Equal(number, obj.getValueAsLong());
        }
    }

    public class DecimalFormValueTests
    {
        [Fact()]
        public void getValueTest()
        {
            decimal val = 123.55M;
            DecimalFormValue obj = new DecimalFormValue(val);

            Xunit.Assert.Equal(val, (decimal) obj.getValue());
            Xunit.Assert.Equal(val, obj.getValueAsDecimal());
        }
    }

    public class BoolFormValueTests
    {
        [Fact()]
        public void getValueTest()
        {
            bool b = true;
            BoolFormValue obj = new BoolFormValue(b);

            Xunit.Assert.Equal(b, (bool) obj.getValue());
            Xunit.Assert.Equal(b, obj.getValueAsBool());
        }
    }

    public class NullFormValueTests
    {
        [Fact()]
        public void getValueTest()
        {
            NullFormValue obj = new NullFormValue();

            Xunit.Assert.Null(obj.getValue());
        }
    }

    public class DictionaryFormValueTests()
    {
        [Fact()]
        public void getValueTest()
        {
            SortedDictionary<string, IFormValue> dict = new SortedDictionary<string, IFormValue>()
            {
                { "name", new StringFormValue("test") },
            };
            DictionaryFormValue obj = new DictionaryFormValue(dict);

            Xunit.Assert.Equal(dict, obj.getValue() as SortedDictionary<string, IFormValue>);
            Xunit.Assert.Equal(dict, obj.getValueAsDictionary());
        }
    }

    public class ArrayListFormValueTests
    {
        [Fact()]
        public void getValueTest()
        {
            ArrayList list = new ArrayList() { "testvalue1" };
            ArrayListFormValue obj = new ArrayListFormValue(list);

            Xunit.Assert.Equal(list, obj.getValue() as ArrayList);
            Xunit.Assert.Equal(list, obj.getValueAsArrayList());
        }
    }
}