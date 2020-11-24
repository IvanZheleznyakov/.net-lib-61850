
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
    [ASN1Sequence ( Name = "Confirmed_ErrorPDU", IsSet = false  )]
    public class Confirmed_ErrorPDU : IASN1PreparedElement {
                    
	private Unsigned32 invokeID_ ;
	
        [ASN1ElementAtr ( Name = "invokeID", IsOptional =  false , HasTag =  true, Tag = 0 , HasDefaultValue =  false )  ]
    
        public Unsigned32 InvokeID
        {
            get { return invokeID_; }
            set { invokeID_ = value;  }
        }
        
                
          
	private Unsigned32 modifierPosition_ ;
	
        private bool  modifierPosition_present = false ;
	
        [ASN1ElementAtr ( Name = "modifierPosition", IsOptional =  true , HasTag =  true, Tag = 1 , HasDefaultValue =  false )  ]
    
        public Unsigned32 ModifierPosition
        {
            get { return modifierPosition_; }
            set { modifierPosition_ = value; modifierPosition_present = true;  }
        }
        
                
          
	private ServiceError serviceError_ ;
	
        [ASN1ElementAtr ( Name = "serviceError", IsOptional =  false , HasTag =  true, Tag = 2 , HasDefaultValue =  false )  ]
    
        public ServiceError ServiceError
        {
            get { return serviceError_; }
            set { serviceError_ = value;  }
        }
        
                
  
        public bool isModifierPositionPresent () {
            return this.modifierPosition_present == true;
        }
        

            public void initWithDefaults() {
            	
            }


            private static IASN1PreparedElementData preparedData = CoderFactory.getInstance().newPreparedElementData(typeof(Confirmed_ErrorPDU));
            public IASN1PreparedElementData PreparedData {
            	get { return preparedData; }
            }

            
    }
            
}
