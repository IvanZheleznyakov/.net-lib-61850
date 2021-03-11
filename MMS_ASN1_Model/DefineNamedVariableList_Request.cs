
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

namespace MMS_ASN1_Model
{


    [ASN1PreparedElement]
    [ASN1Sequence(Name = "DefineNamedVariableList_Request", IsSet = false)]
    public class DefineNamedVariableList_Request : IASN1PreparedElement
    {

        private ObjectName variableListName_;

        [ASN1Element(Name = "variableListName", IsOptional = false, HasTag = false, HasDefaultValue = false)]

        public ObjectName VariableListName
        {
            get { return variableListName_; }
            set { variableListName_ = value; }
        }



        private System.Collections.Generic.ICollection<ListOfVariableSequenceType> listOfVariable_;

        [ASN1PreparedElement]
        [ASN1Sequence(Name = "listOfVariable", IsSet = false)]
        public class ListOfVariableSequenceType : IASN1PreparedElement
        {

            private VariableSpecification variableSpecification_;

            [ASN1Element(Name = "variableSpecification", IsOptional = false, HasTag = false, HasDefaultValue = false)]

            public VariableSpecification VariableSpecification
            {
                get { return variableSpecification_; }
                set { variableSpecification_ = value; }
            }



            private AlternateAccess alternateAccess_;

            private bool alternateAccess_present = false;

            [ASN1Element(Name = "alternateAccess", IsOptional = true, HasTag = true, Tag = 5, HasDefaultValue = false)]

            public AlternateAccess AlternateAccess
            {
                get { return alternateAccess_; }
                set { alternateAccess_ = value; alternateAccess_present = true; }
            }



            public bool isAlternateAccessPresent()
            {
                return this.alternateAccess_present == true;
            }


            public void initWithDefaults()
            {

            }

            private static IASN1PreparedElementData preparedData = CoderFactory.getInstance().newPreparedElementData(typeof(ListOfVariableSequenceType));
            public IASN1PreparedElementData PreparedData
            {
                get { return preparedData; }
            }


        }

        [ASN1SequenceOf(Name = "listOfVariable", IsSetOf = false)]


        [ASN1Element(Name = "listOfVariable", IsOptional = false, HasTag = true, Tag = 0, HasDefaultValue = false)]

        public System.Collections.Generic.ICollection<ListOfVariableSequenceType> ListOfVariable
        {
            get { return listOfVariable_; }
            set { listOfVariable_ = value; }
        }




        public void initWithDefaults()
        {

        }


        private static IASN1PreparedElementData preparedData = CoderFactory.getInstance().newPreparedElementData(typeof(DefineNamedVariableList_Request));
        public IASN1PreparedElementData PreparedData
        {
            get { return preparedData; }
        }


    }

}
