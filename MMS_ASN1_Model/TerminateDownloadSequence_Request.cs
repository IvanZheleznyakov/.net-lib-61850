
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
    [ASN1Sequence ( Name = "TerminateDownloadSequence_Request", IsSet = false  )]
    public class TerminateDownloadSequence_Request : IASN1PreparedElement {
                    
	private Identifier domainName_ ;
	
        [ASN1ElementAtr ( Name = "domainName", IsOptional =  false , HasTag =  true, Tag = 0 , HasDefaultValue =  false )  ]
    
        public Identifier DomainName
        {
            get { return domainName_; }
            set { domainName_ = value;  }
        }
        
                
          
	private ServiceError discard_ ;
	
        private bool  discard_present = false ;
	
        [ASN1ElementAtr ( Name = "discard", IsOptional =  true , HasTag =  true, Tag = 1 , HasDefaultValue =  false )  ]
    
        public ServiceError Discard
        {
            get { return discard_; }
            set { discard_ = value; discard_present = true;  }
        }
        
                
  
        public bool isDiscardPresent () {
            return this.discard_present == true;
        }
        

            public void initWithDefaults() {
            	
            }


            private static IASN1PreparedElementData preparedData = CoderFactory.getInstance().newPreparedElementData(typeof(TerminateDownloadSequence_Request));
            public IASN1PreparedElementData PreparedData {
            	get { return preparedData; }
            }

            
    }
            
}
