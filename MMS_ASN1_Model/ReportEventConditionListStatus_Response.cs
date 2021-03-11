
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

namespace MMS_ASN1_Model {


    [ASN1PreparedElement]
    [ASN1Sequence ( Name = "ReportEventConditionListStatus_Response", IsSet = false  )]
    public class ReportEventConditionListStatus_Response : IASN1PreparedElement {
                    
	private System.Collections.Generic.ICollection<EventConditionStatus> listOfEventConditionStatus_ ;
	
[ASN1SequenceOf( Name = "listOfEventConditionStatus", IsSetOf = false  )]

    
        [ASN1Element ( Name = "listOfEventConditionStatus", IsOptional =  false , HasTag =  true, Tag = 1 , HasDefaultValue =  false )  ]
    
        public System.Collections.Generic.ICollection<EventConditionStatus> ListOfEventConditionStatus
        {
            get { return listOfEventConditionStatus_; }
            set { listOfEventConditionStatus_ = value;  }
        }
        
                
          
	private bool moreFollows_ ;
	[ASN1Boolean( Name = "" )]
    
        [ASN1Element ( Name = "moreFollows", IsOptional =  false , HasTag =  true, Tag = 2 , HasDefaultValue =  true )  ]
    
        public bool MoreFollows
        {
            get { return moreFollows_; }
            set { moreFollows_ = value;  }
        }
        
                
  

            public void initWithDefaults() {
            	bool param_MoreFollows =
            false;
        MoreFollows = param_MoreFollows;
    
            }


            private static IASN1PreparedElementData preparedData = CoderFactory.getInstance().newPreparedElementData(typeof(ReportEventConditionListStatus_Response));
            public IASN1PreparedElementData PreparedData {
            	get { return preparedData; }
            }

            
    }
            
}
