
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
    [ASN1Sequence ( Name = "RemoveEventConditionListReference_Request", IsSet = false  )]
    public class RemoveEventConditionListReference_Request : IASN1PreparedElement {
                    
	private ObjectName eventConditionListName_ ;
	
        [ASN1Element ( Name = "eventConditionListName", IsOptional =  false , HasTag =  true, Tag = 0 , HasDefaultValue =  false )  ]
    
        public ObjectName EventConditionListName
        {
            get { return eventConditionListName_; }
            set { eventConditionListName_ = value;  }
        }
        
                
          
	private System.Collections.Generic.ICollection<ObjectName> listOfEventConditionName_ ;
	
[ASN1SequenceOf( Name = "listOfEventConditionName", IsSetOf = false  )]

    
        [ASN1Element ( Name = "listOfEventConditionName", IsOptional =  false , HasTag =  true, Tag = 1 , HasDefaultValue =  false )  ]
    
        public System.Collections.Generic.ICollection<ObjectName> ListOfEventConditionName
        {
            get { return listOfEventConditionName_; }
            set { listOfEventConditionName_ = value;  }
        }
        
                
          
	private System.Collections.Generic.ICollection<ObjectName> listOfEventConditionListName_ ;
	
[ASN1SequenceOf( Name = "listOfEventConditionListName", IsSetOf = false  )]

    
        [ASN1Element ( Name = "listOfEventConditionListName", IsOptional =  false , HasTag =  true, Tag = 2 , HasDefaultValue =  false )  ]
    
        public System.Collections.Generic.ICollection<ObjectName> ListOfEventConditionListName
        {
            get { return listOfEventConditionListName_; }
            set { listOfEventConditionListName_ = value;  }
        }
        
                
  

            public void initWithDefaults() {
            	
            }


            private static IASN1PreparedElementData preparedData = CoderFactory.getInstance().newPreparedElementData(typeof(RemoveEventConditionListReference_Request));
            public IASN1PreparedElementData PreparedData {
            	get { return preparedData; }
            }

            
    }
            
}
