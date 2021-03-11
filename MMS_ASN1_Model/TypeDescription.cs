
//
// This file was generated by the BinaryNotes compiler.
// See http://bnotes.sourceforge.net 
// Any modifications to this file will be lost upon recompilation of the source ASN.1. 
//

using System;
using org.bn.attributes;
using org.bn.attributes.constraints;
using org.bn.coders;
using org.bn.types;
using org.bn;
using lib61850net;

namespace MMS_ASN1_Model
{


    [ASN1PreparedElement]
    [ASN1Choice(Name = "TypeDescription")]
    public class TypeDescription : IASN1PreparedElement
    {


        private ArraySequenceType array_;
        private bool array_selected = false;

        internal MmsTypeEnum MmsType { get; set; } = MmsTypeEnum.UNKNOWN;


        [ASN1PreparedElement]
        [ASN1Sequence(Name = "array", IsSet = false)]
        public class ArraySequenceType : IASN1PreparedElement
        {

            private bool packed_;
            [ASN1Boolean(Name = "packed")]
            [ASN1Element(Name = "packed", IsOptional = false, HasTag = true, Tag = 0, HasDefaultValue = true)]
            public bool Packed
            {
                get { return packed_; }
                set { packed_ = value; }
            }



            private Unsigned32 numberOfElements_;

            [ASN1Element(Name = "numberOfElements", IsOptional = false, HasTag = true, Tag = 1, HasDefaultValue = false)]
            public Unsigned32 NumberOfElements
            {
                get { return numberOfElements_; }
                set { numberOfElements_ = value; }
            }



            private TypeSpecification elementType_;

            [ASN1Element(Name = "elementType", IsOptional = false, HasTag = true, Tag = 2, HasDefaultValue = false)]
            public TypeSpecification ElementType
            {
                get { return elementType_; }
                set { elementType_ = value; }
            }




            public void initWithDefaults()
            {
                bool param_Packed =
        false;
                Packed = param_Packed;

            }

            private static IASN1PreparedElementData preparedData = CoderFactory.getInstance().newPreparedElementData(typeof(ArraySequenceType));
            public IASN1PreparedElementData PreparedData
            {
                get { return preparedData; }
            }


        }

        [ASN1Element(Name = "array", IsOptional = false, HasTag = true, Tag = 1, HasDefaultValue = false)]
        public ArraySequenceType Array
        {
            get { return array_; }
            set { selectArray(value); }
        }




        private StructureSequenceType structure_;
        private bool structure_selected = false;



        [ASN1PreparedElement]
        [ASN1Sequence(Name = "structure", IsSet = false)]
        public class StructureSequenceType : IASN1PreparedElement
        {

            private bool packed_;
            [ASN1Boolean(Name = "")]

            [ASN1Element(Name = "packed", IsOptional = false, HasTag = true, Tag = 0, HasDefaultValue = true)]

            public bool Packed
            {
                get { return packed_; }
                set { packed_ = value; }
            }



            private System.Collections.Generic.ICollection<ComponentsSequenceType> components_;

            [ASN1PreparedElement]
            [ASN1Sequence(Name = "components", IsSet = false)]
            public class ComponentsSequenceType : IASN1PreparedElement
            {

                private Identifier componentName_;

                private bool componentName_present = false;

                [ASN1Element(Name = "componentName", IsOptional = true, HasTag = true, Tag = 0, HasDefaultValue = false)]

                public Identifier ComponentName
                {
                    get { return componentName_; }
                    set { componentName_ = value; componentName_present = true; }
                }



                private TypeSpecification componentType_;

                [ASN1Element(Name = "componentType", IsOptional = false, HasTag = true, Tag = 1, HasDefaultValue = false)]

                public TypeSpecification ComponentType
                {
                    get { return componentType_; }
                    set { componentType_ = value; }
                }



                public bool isComponentNamePresent()
                {
                    return this.componentName_present == true;
                }


                public void initWithDefaults()
                {

                }

                private static IASN1PreparedElementData preparedData = CoderFactory.getInstance().newPreparedElementData(typeof(ComponentsSequenceType));
                public IASN1PreparedElementData PreparedData
                {
                    get { return preparedData; }
                }


            }

            [ASN1SequenceOf(Name = "components", IsSetOf = false)]


            [ASN1Element(Name = "components", IsOptional = false, HasTag = true, Tag = 1, HasDefaultValue = false)]

            public System.Collections.Generic.ICollection<ComponentsSequenceType> Components
            {
                get { return components_; }
                set { components_ = value; }
            }




            public void initWithDefaults()
            {
                bool param_Packed =
        false;
                Packed = param_Packed;

            }

            private static IASN1PreparedElementData preparedData = CoderFactory.getInstance().newPreparedElementData(typeof(StructureSequenceType));
            public IASN1PreparedElementData PreparedData
            {
                get { return preparedData; }
            }


        }

        [ASN1Element(Name = "structure", IsOptional = false, HasTag = true, Tag = 2, HasDefaultValue = false)]

        public StructureSequenceType Structure
        {
            get { return structure_; }
            set { selectStructure(value); }
        }




        private NullObject boolean_;
        private bool boolean_selected = false;



        [ASN1Null(Name = "boolean")]

        [ASN1Element(Name = "boolean", IsOptional = false, HasTag = true, Tag = 3, HasDefaultValue = false)]

        public NullObject Boolean
        {
            get { return boolean_; }
            set { selectBoolean(value); }
        }




        private Integer32 bit_string_;
        private bool bit_string_selected = false;



        [ASN1Element(Name = "bit-string", IsOptional = false, HasTag = true, Tag = 4, HasDefaultValue = false)]

        public Integer32 Bit_string
        {
            get { return bit_string_; }
            set { selectBit_string(value); }
        }




        private Unsigned8 integer_;
        private bool integer_selected = false;



        [ASN1Element(Name = "integer", IsOptional = false, HasTag = true, Tag = 5, HasDefaultValue = false)]

        public Unsigned8 Integer
        {
            get { return integer_; }
            set { selectInteger(value); }
        }




        private Unsigned8 unsigned_;
        private bool unsigned_selected = false;



        [ASN1Element(Name = "unsigned", IsOptional = false, HasTag = true, Tag = 6, HasDefaultValue = false)]

        public Unsigned8 Unsigned
        {
            get { return unsigned_; }
            set { selectUnsigned(value); }
        }




        private Floating_pointSequenceType floating_point_;
        private bool floating_point_selected = false;



        [ASN1PreparedElement]
        [ASN1Sequence(Name = "floating-point", IsSet = false)]
        public class Floating_pointSequenceType : IASN1PreparedElement
        {

            private Unsigned8 format_width_;

            [ASN1Element(Name = "format-width", IsOptional = false, HasTag = false, HasDefaultValue = false)]

            public Unsigned8 Format_width
            {
                get { return format_width_; }
                set { format_width_ = value; }
            }



            private Unsigned8 exponent_width_;

            [ASN1Element(Name = "exponent-width", IsOptional = false, HasTag = false, HasDefaultValue = false)]

            public Unsigned8 Exponent_width
            {
                get { return exponent_width_; }
                set { exponent_width_ = value; }
            }




            public void initWithDefaults()
            {

            }

            private static IASN1PreparedElementData preparedData = CoderFactory.getInstance().newPreparedElementData(typeof(Floating_pointSequenceType));
            public IASN1PreparedElementData PreparedData
            {
                get { return preparedData; }
            }


        }

        [ASN1Element(Name = "floating-point", IsOptional = false, HasTag = true, Tag = 7, HasDefaultValue = false)]

        public Floating_pointSequenceType Floating_point
        {
            get { return floating_point_; }
            set { selectFloating_point(value); }
        }




        private Integer32 octet_string_;
        private bool octet_string_selected = false;



        [ASN1Element(Name = "octet-string", IsOptional = false, HasTag = true, Tag = 9, HasDefaultValue = false)]

        public Integer32 Octet_string
        {
            get { return octet_string_; }
            set { selectOctet_string(value); }
        }




        private Integer32 visible_string_;
        private bool visible_string_selected = false;



        [ASN1Element(Name = "visible-string", IsOptional = false, HasTag = true, Tag = 10, HasDefaultValue = false)]

        public Integer32 Visible_string
        {
            get { return visible_string_; }
            set { selectVisible_string(value); }
        }




        private NullObject generalized_time_;
        private bool generalized_time_selected = false;



        [ASN1Null(Name = "generalized-time")]

        [ASN1Element(Name = "generalized-time", IsOptional = false, HasTag = true, Tag = 11, HasDefaultValue = false)]

        public NullObject Generalized_time
        {
            get { return generalized_time_; }
            set { selectGeneralized_time(value); }
        }




        private bool binary_time_;
        private bool binary_time_selected = false;


        [ASN1Boolean(Name = "binary-time")]

        [ASN1Element(Name = "binary-time", IsOptional = false, HasTag = true, Tag = 12, HasDefaultValue = false)]

        public bool Binary_time
        {
            get { return binary_time_; }
            set { selectBinary_time(value); }
        }




        private Unsigned8 bcd_;
        private bool bcd_selected = false;



        [ASN1Element(Name = "bcd", IsOptional = false, HasTag = true, Tag = 13, HasDefaultValue = false)]

        public Unsigned8 Bcd
        {
            get { return bcd_; }
            set { selectBcd(value); }
        }




        private NullObject objId_;
        private bool objId_selected = false;



        [ASN1Null(Name = "objId")]

        [ASN1Element(Name = "objId", IsOptional = false, HasTag = true, Tag = 15, HasDefaultValue = false)]

        public NullObject ObjId
        {
            get { return objId_; }
            set { selectObjId(value); }
        }




        private Integer32 mMSString_;
        private bool mMSString_selected = false;



        [ASN1Element(Name = "mMSString", IsOptional = false, HasTag = true, Tag = 16, HasDefaultValue = false)]

        public Integer32 MMSString
        {
            get { return mMSString_; }
            set { selectMMSString(value); }
        }




        private UtcTime utc_time_;
        private bool utc_time_selected = false;



        [ASN1Element(Name = "utc-time", IsOptional = false, HasTag = true, Tag = 17, HasDefaultValue = false)]

        public UtcTime Utc_time
        {
            get { return utc_time_; }
            set { selectUtc_time(value); }
        }



        public bool isArraySelected()
        {
            return this.array_selected;
        }




        public void selectArray(ArraySequenceType val)
        {
            this.array_ = val;
            this.array_selected = true;
            MmsType = MmsTypeEnum.ARRAY;


            this.structure_selected = false;

            this.boolean_selected = false;

            this.bit_string_selected = false;

            this.integer_selected = false;

            this.unsigned_selected = false;

            this.floating_point_selected = false;

            this.octet_string_selected = false;

            this.visible_string_selected = false;

            this.generalized_time_selected = false;

            this.binary_time_selected = false;

            this.bcd_selected = false;

            this.objId_selected = false;

            this.mMSString_selected = false;

            this.utc_time_selected = false;

        }


        public bool isStructureSelected()
        {
            return this.structure_selected;
        }




        public void selectStructure(StructureSequenceType val)
        {
            this.structure_ = val;
            this.structure_selected = true;
            MmsType = MmsTypeEnum.STRUCTURE;


            this.array_selected = false;

            this.boolean_selected = false;

            this.bit_string_selected = false;

            this.integer_selected = false;

            this.unsigned_selected = false;

            this.floating_point_selected = false;

            this.octet_string_selected = false;

            this.visible_string_selected = false;

            this.generalized_time_selected = false;

            this.binary_time_selected = false;

            this.bcd_selected = false;

            this.objId_selected = false;

            this.mMSString_selected = false;

            this.utc_time_selected = false;

        }


        public bool isBooleanSelected()
        {
            return this.boolean_selected;
        }


        /*public void selectBoolean()
        {
            selectBoolean(new NullObject());
        }*/



        public void selectBoolean(NullObject val)
        {
            this.boolean_ = val;
            this.boolean_selected = true;
            MmsType = MmsTypeEnum.BOOLEAN;


            this.array_selected = false;

            this.structure_selected = false;

            this.bit_string_selected = false;

            this.integer_selected = false;

            this.unsigned_selected = false;

            this.floating_point_selected = false;

            this.octet_string_selected = false;

            this.visible_string_selected = false;

            this.generalized_time_selected = false;

            this.binary_time_selected = false;

            this.bcd_selected = false;

            this.objId_selected = false;

            this.mMSString_selected = false;

            this.utc_time_selected = false;

        }


        public bool isBit_stringSelected()
        {
            return this.bit_string_selected;
        }




        public void selectBit_string(Integer32 val)
        {
            this.bit_string_ = val;
            this.bit_string_selected = true;
            MmsType = MmsTypeEnum.BIT_STRING;


            this.array_selected = false;

            this.structure_selected = false;

            this.boolean_selected = false;

            this.integer_selected = false;

            this.unsigned_selected = false;

            this.floating_point_selected = false;

            this.octet_string_selected = false;

            this.visible_string_selected = false;

            this.generalized_time_selected = false;

            this.binary_time_selected = false;

            this.bcd_selected = false;

            this.objId_selected = false;

            this.mMSString_selected = false;

            this.utc_time_selected = false;

        }


        public bool isIntegerSelected()
        {
            return this.integer_selected;
        }




        public void selectInteger(Unsigned8 val)
        {
            this.integer_ = val;
            this.integer_selected = true;
            MmsType = MmsTypeEnum.INTEGER;


            this.array_selected = false;

            this.structure_selected = false;

            this.boolean_selected = false;

            this.bit_string_selected = false;

            this.unsigned_selected = false;

            this.floating_point_selected = false;

            this.octet_string_selected = false;

            this.visible_string_selected = false;

            this.generalized_time_selected = false;

            this.binary_time_selected = false;

            this.bcd_selected = false;

            this.objId_selected = false;

            this.mMSString_selected = false;

            this.utc_time_selected = false;

        }


        public bool isUnsignedSelected()
        {
            return this.unsigned_selected;
        }




        public void selectUnsigned(Unsigned8 val)
        {
            this.unsigned_ = val;
            this.unsigned_selected = true;
            MmsType = MmsTypeEnum.UNSIGNED;


            this.array_selected = false;

            this.structure_selected = false;

            this.boolean_selected = false;

            this.bit_string_selected = false;

            this.integer_selected = false;

            this.floating_point_selected = false;

            this.octet_string_selected = false;

            this.visible_string_selected = false;

            this.generalized_time_selected = false;

            this.binary_time_selected = false;

            this.bcd_selected = false;

            this.objId_selected = false;

            this.mMSString_selected = false;

            this.utc_time_selected = false;

        }


        public bool isFloating_pointSelected()
        {
            return this.floating_point_selected;
        }




        public void selectFloating_point(Floating_pointSequenceType val)
        {
            this.floating_point_ = val;
            this.floating_point_selected = true;
            MmsType = MmsTypeEnum.FLOATING_POINT;


            this.array_selected = false;

            this.structure_selected = false;

            this.boolean_selected = false;

            this.bit_string_selected = false;

            this.integer_selected = false;

            this.unsigned_selected = false;

            this.octet_string_selected = false;

            this.visible_string_selected = false;

            this.generalized_time_selected = false;

            this.binary_time_selected = false;

            this.bcd_selected = false;

            this.objId_selected = false;

            this.mMSString_selected = false;

            this.utc_time_selected = false;

        }


        public bool isOctet_stringSelected()
        {
            return this.octet_string_selected;
        }




        public void selectOctet_string(Integer32 val)
        {
            this.octet_string_ = val;
            this.octet_string_selected = true;
            MmsType = MmsTypeEnum.OCTET_STRING;


            this.array_selected = false;

            this.structure_selected = false;

            this.boolean_selected = false;

            this.bit_string_selected = false;

            this.integer_selected = false;

            this.unsigned_selected = false;

            this.floating_point_selected = false;

            this.visible_string_selected = false;

            this.generalized_time_selected = false;

            this.binary_time_selected = false;

            this.bcd_selected = false;

            this.objId_selected = false;

            this.mMSString_selected = false;

            this.utc_time_selected = false;

        }


        public bool isVisible_stringSelected()
        {
            return this.visible_string_selected;
        }




        public void selectVisible_string(Integer32 val)
        {
            this.visible_string_ = val;
            this.visible_string_selected = true;
            MmsType = MmsTypeEnum.VISIBLE_STRING;


            this.array_selected = false;

            this.structure_selected = false;

            this.boolean_selected = false;

            this.bit_string_selected = false;

            this.integer_selected = false;

            this.unsigned_selected = false;

            this.floating_point_selected = false;

            this.octet_string_selected = false;

            this.generalized_time_selected = false;

            this.binary_time_selected = false;

            this.bcd_selected = false;

            this.objId_selected = false;

            this.mMSString_selected = false;

            this.utc_time_selected = false;

        }


        public bool isGeneralized_timeSelected()
        {
            return this.generalized_time_selected;
        }


        /*public void selectGeneralized_time()
        {
            selectGeneralized_time(new NullObject());
        }*/



        public void selectGeneralized_time(NullObject val)
        {
            this.generalized_time_ = val;
            this.generalized_time_selected = true;
            MmsType = MmsTypeEnum.GENERALIZED_TIME;


            this.array_selected = false;

            this.structure_selected = false;

            this.boolean_selected = false;

            this.bit_string_selected = false;

            this.integer_selected = false;

            this.unsigned_selected = false;

            this.floating_point_selected = false;

            this.octet_string_selected = false;

            this.visible_string_selected = false;

            this.binary_time_selected = false;

            this.bcd_selected = false;

            this.objId_selected = false;

            this.mMSString_selected = false;

            this.utc_time_selected = false;

        }


        public bool isBinary_timeSelected()
        {
            return this.binary_time_selected;
        }




        public void selectBinary_time(bool val)
        {
            this.binary_time_ = val;
            this.binary_time_selected = true;
            MmsType = MmsTypeEnum.BINARY_TIME;


            this.array_selected = false;

            this.structure_selected = false;

            this.boolean_selected = false;

            this.bit_string_selected = false;

            this.integer_selected = false;

            this.unsigned_selected = false;

            this.floating_point_selected = false;

            this.octet_string_selected = false;

            this.visible_string_selected = false;

            this.generalized_time_selected = false;

            this.bcd_selected = false;

            this.objId_selected = false;

            this.mMSString_selected = false;

            this.utc_time_selected = false;

        }


        public bool isBcdSelected()
        {
            return this.bcd_selected;
        }




        public void selectBcd(Unsigned8 val)
        {
            this.bcd_ = val;
            this.bcd_selected = true;
            MmsType = MmsTypeEnum.BCD;


            this.array_selected = false;

            this.structure_selected = false;

            this.boolean_selected = false;

            this.bit_string_selected = false;

            this.integer_selected = false;

            this.unsigned_selected = false;

            this.floating_point_selected = false;

            this.octet_string_selected = false;

            this.visible_string_selected = false;

            this.generalized_time_selected = false;

            this.binary_time_selected = false;

            this.objId_selected = false;

            this.mMSString_selected = false;

            this.utc_time_selected = false;

        }


        public bool isObjIdSelected()
        {
            return this.objId_selected;
        }


       /* public void selectObjId()
        {
            selectObjId(new NullObject());
        }*/



        public void selectObjId(NullObject val)
        {
            this.objId_ = val;
            this.objId_selected = true;
            MmsType = MmsTypeEnum.OBJ_ID;


            this.array_selected = false;

            this.structure_selected = false;

            this.boolean_selected = false;

            this.bit_string_selected = false;

            this.integer_selected = false;

            this.unsigned_selected = false;

            this.floating_point_selected = false;

            this.octet_string_selected = false;

            this.visible_string_selected = false;

            this.generalized_time_selected = false;

            this.binary_time_selected = false;

            this.bcd_selected = false;

            this.mMSString_selected = false;

            this.utc_time_selected = false;

        }


        public bool isMMSStringSelected()
        {
            return this.mMSString_selected;
        }




        public void selectMMSString(Integer32 val)
        {
            this.mMSString_ = val;
            this.mMSString_selected = true;
            MmsType = MmsTypeEnum.MMS_STRING;


            this.array_selected = false;

            this.structure_selected = false;

            this.boolean_selected = false;

            this.bit_string_selected = false;

            this.integer_selected = false;

            this.unsigned_selected = false;

            this.floating_point_selected = false;

            this.octet_string_selected = false;

            this.visible_string_selected = false;

            this.generalized_time_selected = false;

            this.binary_time_selected = false;

            this.bcd_selected = false;

            this.objId_selected = false;

            this.utc_time_selected = false;

        }


        public bool isUtc_timeSelected()
        {
            return this.utc_time_selected;
        }




        public void selectUtc_time(UtcTime val)
        {
            this.utc_time_ = val;
            this.utc_time_selected = true;
            MmsType = MmsTypeEnum.UTC_TIME;


            this.array_selected = false;

            this.structure_selected = false;

            this.boolean_selected = false;

            this.bit_string_selected = false;

            this.integer_selected = false;

            this.unsigned_selected = false;

            this.floating_point_selected = false;

            this.octet_string_selected = false;

            this.visible_string_selected = false;

            this.generalized_time_selected = false;

            this.binary_time_selected = false;

            this.bcd_selected = false;

            this.objId_selected = false;

            this.mMSString_selected = false;

        }



        public void initWithDefaults()
        {
        }

        private static IASN1PreparedElementData preparedData = CoderFactory.getInstance().newPreparedElementData(typeof(TypeDescription));
        public IASN1PreparedElementData PreparedData
        {
            get { return preparedData; }
        }

    }

}
