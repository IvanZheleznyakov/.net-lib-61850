
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
    [ASN1BoxedType(Name = "Write_Response")]
    public class Write_Response : IASN1PreparedElement
    {

        private System.Collections.Generic.ICollection<Write_ResponseChoiceType> val = null;



        [ASN1PreparedElement]
        [ASN1Choice(Name = "Write-Response")]
        public class Write_ResponseChoiceType : IASN1PreparedElement
        {


            private DataAccessError failure_;
            private bool failure_selected = false;



            [ASN1ElementAtr(Name = "failure", IsOptional = false, HasTag = true, Tag = 0, HasDefaultValue = false)]

            public DataAccessError Failure
            {
                get { return failure_; }
                set { selectFailure(value); }
            }




            private NullObject success_;
            private bool success_selected = false;



            [ASN1Null(Name = "success")]

            [ASN1ElementAtr(Name = "success", IsOptional = false, HasTag = true, Tag = 1, HasDefaultValue = false)]

            public NullObject Success
            {
                get { return success_; }
                set { selectSuccess(value); }
            }



            public bool isFailureSelected()
            {
                return this.failure_selected;
            }




            public void selectFailure(DataAccessError val)
            {
                this.failure_ = val;
                this.failure_selected = true;


                this.success_selected = false;

            }


            public bool isSuccessSelected()
            {
                return this.success_selected;
            }


            /*public void selectSuccess()
            {
                selectSuccess(new NullObject());
            }*/



            public void selectSuccess(NullObject val)
            {
                this.success_ = val;
                this.success_selected = true;


                this.failure_selected = false;

            }



            public void initWithDefaults()
            {
            }

            private static IASN1PreparedElementData preparedData = CoderFactory.getInstance().newPreparedElementData(typeof(Write_ResponseChoiceType));
            public IASN1PreparedElementData PreparedData
            {
                get { return preparedData; }
            }

        }

        [ASN1SequenceOf(Name = "Write-Response", IsSetOf = false)]

        public System.Collections.Generic.ICollection<Write_ResponseChoiceType> Value
        {
            get { return val; }
            set { val = value; }
        }

        public void initValue()
        {
            this.Value = new System.Collections.Generic.List<Write_ResponseChoiceType>();
        }

        public void Add(Write_ResponseChoiceType item)
        {
            this.Value.Add(item);
        }

        public void initWithDefaults()
        {
        }


        private static IASN1PreparedElementData preparedData = CoderFactory.getInstance().newPreparedElementData(typeof(Write_Response));
        public IASN1PreparedElementData PreparedData
        {
            get { return preparedData; }
        }

    }

}
