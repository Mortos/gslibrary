using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace gslibrary.BasicTypes
{
    public class BaseStruct
    {
        public virtual string[] members() 
        {
            return new string[] {  };
        }

        public virtual void AsText(StringBuilder b, int pad)
        {
        }

        public virtual string AsText()
        {
            /*
            var stringPropertyNamesAndValues = this.GetType();
            var fields = this.GetType().GetFields();
            var props = this.GetType().GetProperties();
            var propsw = props.Where(pi => pi.PropertyType == typeof(string) && pi.GetGetMethod() != null)
            .Select(pi => new
            {
                Name = pi.Name,
                Value = pi.GetGetMethod().Invoke(this, null)
            });
            */
            var b = new StringBuilder();
            AsText(b, 0);
            return b.ToString();
        }

        public virtual void Parse(GameBitBuffer buffer)
        {
            throw new NotImplementedException("");
        }
        public virtual void Encode(GameBitBuffer buffer)
        {
            throw new NotImplementedException("");
        }
    }
}
