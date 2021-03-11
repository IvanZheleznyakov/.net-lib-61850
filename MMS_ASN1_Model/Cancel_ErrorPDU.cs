
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
    [ASN1Sequence ( Name = "Cancel_ErrorPDU", IsSet = false  )]
    public class Cancel_ErrorPDU : IASN1PreparedElement {
                    
	private Unsigned32 originalInvokeID_ ;
	
        [ASN1Element ( Name = "originalInvokeID", IsOptional =  false , HasTag =  true, Tag = 0 , HasDefaultValue =  false )  ]
    
        public Unsigned32 OriginalInvokeID
        {
            get { return originalInvokeID_; }
            set { originalInvokeID_ = value;  }
        }
        
                
          
	private ServiceError serviceError_ ;
	
        [ASN1Element ( Name = "serviceError", IsOptional =  false , HasTag =  true, Tag = 1 , HasDefaultValue =  false )  ]
    
        public ServiceError ServiceError
        {
            get { return serviceError_; }
            set { serviceError_ = value;  }
        }
        
                
  

            public void initWithDefaults() {
            	
            }


            private static IASN1PreparedElementData preparedData = CoderFactory.getInstance().newPreparedElementData(typeof(Cancel_ErrorPDU));
            public IASN1PreparedElementData PreparedData {
            	get { return preparedData; }
            }

            
    }
            
}
