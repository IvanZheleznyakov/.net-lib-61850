
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
    [ASN1Sequence(Name = "CreateProgramInvocation_Request", IsSet = false)]
    public class CreateProgramInvocation_Request : IASN1PreparedElement
    {

        private Identifier programInvocationName_;

        [ASN1ElementAtr(Name = "programInvocationName", IsOptional = false, HasTag = true, Tag = 0, HasDefaultValue = false)]

        public Identifier ProgramInvocationName
        {
            get { return programInvocationName_; }
            set { programInvocationName_ = value; }
        }



        private System.Collections.Generic.ICollection<Identifier> listOfDomainNames_;

        [ASN1SequenceOf(Name = "listOfDomainNames", IsSetOf = false)]


        [ASN1ElementAtr(Name = "listOfDomainNames", IsOptional = false, HasTag = true, Tag = 1, HasDefaultValue = false)]

        public System.Collections.Generic.ICollection<Identifier> ListOfDomainNames
        {
            get { return listOfDomainNames_; }
            set { listOfDomainNames_ = value; }
        }



        private bool reusable_;
        [ASN1Boolean(Name = "")]

        [ASN1ElementAtr(Name = "reusable", IsOptional = false, HasTag = true, Tag = 2, HasDefaultValue = true)]

        public bool Reusable
        {
            get { return reusable_; }
            set { reusable_ = value; }
        }



        private bool monitorType_;

        private bool monitorType_present = false;
        [ASN1Boolean(Name = "")]

        [ASN1ElementAtr(Name = "monitorType", IsOptional = true, HasTag = true, Tag = 3, HasDefaultValue = false)]

        public bool MonitorType
        {
            get { return monitorType_; }
            set { monitorType_ = value; monitorType_present = true; }
        }



        public bool isMonitorTypePresent()
        {
            return this.monitorType_present == true;
        }


        public void initWithDefaults()
        {
            bool param_Reusable =
        false;
            Reusable = param_Reusable;

        }


        private static IASN1PreparedElementData preparedData = CoderFactory.getInstance().newPreparedElementData(typeof(CreateProgramInvocation_Request));
        public IASN1PreparedElementData PreparedData
        {
            get { return preparedData; }
        }


    }

}
