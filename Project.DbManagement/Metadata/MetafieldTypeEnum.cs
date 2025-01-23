using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Project.DbManagement
{
    public enum MetafieldTypeEnum
    {
        [XmlEnum("0")]
        Number = 0,
        [XmlEnum("1")]
        Bit = 1,
        [XmlEnum("4")]
        DateTime = 4,
        [XmlEnum("7")]
        Guid = 7,
        [XmlEnum("14")]
        XDateTime = 14,
        [XmlEnum("15")]
        SmallDateTime = 15,
        [XmlEnum("18")]
        Text = 18,
        [XmlEnum("20")]
        TextArea = 20,
        [XmlEnum("28")]
        ShortDate = 28,
        [XmlEnum("29")]
        Email = 29,
        [XmlEnum("30")]
        URL = 30,
        [XmlEnum("41")]
        Dropdownlist = 41,
        [XmlEnum("44")]
        Radio = 44,
        [XmlEnum("49")]
        CheckboxList = 49,
        [XmlEnum("51")]
        UserMultiChoice = 51,
        [XmlEnum("53")]
        UserRadio = 53,
        [XmlEnum("56")]
        UserCheckboxList = 56,
        [XmlEnum("59")]
        UserDropdownList = 59,
        [XmlEnum("81")]
        NodeBaseMultiChoice = 81,
        [XmlEnum("83")]
        NodeBaseRadio = 83,
        [XmlEnum("86")]
        NodeBaseCheckboxList = 86,
        [XmlEnum("89")]
        NodeBaseDropdownlist = 89,
        [XmlEnum("91")]
        BaseCategoryMultiChoice = 91,
        [XmlEnum("93")]
        BaseCategoryRadio = 93,
        [XmlEnum("96")]
        BaseCategoryCheckboxList = 96,
        [XmlEnum("99")]
        BaseCategoryDropdownlist = 99
    }
}
