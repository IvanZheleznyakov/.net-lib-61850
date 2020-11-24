
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
    [ASN1Sequence ( Name = "AddToUnitControl_Request", IsSet = false  )]
    public class AddToUnitControl_Request : IASN1PreparedElement {
                    
	private Identifier unitControl_ ;
	
        [ASN1ElementAtr ( Name = "unitControl", IsOptional =  false , HasTag =  true, Tag = 0 , HasDefaultValue =  false )  ]
    
        public Identifier UnitControl
        {
            get { return unitControl_; }
            set { unitControl_ = value;  }
        }
        
                
          
	private System.Collections.Generic.ICollection<Identifier> domains_ ;
	
[ASN1SequenceOf( Name = "domains", IsSetOf = false  )]

    
        [ASN1ElementAtr ( Name = "domains", IsOptional =  false , HasTag =  true, Tag = 1 , HasDefaultValue =  false )  ]
    
        public System.Collections.Generic.ICollection<Identifier> Domains
        {
            get { return domains_; }
            set { domains_ = value;  }
        }
        
                
          
	private System.Collections.Generic.ICollection<Identifier> programInvocations_ ;
	
[ASN1SequenceOf( Name = "programInvocations", IsSetOf = false  )]

    
        [ASN1ElementAtr ( Name = "programInvocations", IsOptional =  false , HasTag =  true, Tag = 2 , HasDefaultValue =  false )  ]
    
        public System.Collections.Generic.ICollection<Identifier> ProgramInvocations
        {
            get { return programInvocations_; }
            set { programInvocations_ = value;  }
        }
        
                
  

            public void initWithDefaults() {
            	
            }


            private static IASN1PreparedElementData preparedData = CoderFactory.getInstance().newPreparedElementData(typeof(AddToUnitControl_Request));
            public IASN1PreparedElementData PreparedData {
            	get { return preparedData; }
            }

            
    }
            
}
