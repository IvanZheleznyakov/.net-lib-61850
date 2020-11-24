
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
    [ASN1Sequence ( Name = "StopUnitControl_Error", IsSet = false  )]
    public class StopUnitControl_Error : IASN1PreparedElement {
                    
	private Identifier programInvocationName_ ;
	
        [ASN1ElementAtr ( Name = "programInvocationName", IsOptional =  false , HasTag =  true, Tag = 0 , HasDefaultValue =  false )  ]
    
        public Identifier ProgramInvocationName
        {
            get { return programInvocationName_; }
            set { programInvocationName_ = value;  }
        }
        
                
          
	private ProgramInvocationState programInvocationState_ ;
	
        [ASN1ElementAtr ( Name = "programInvocationState", IsOptional =  false , HasTag =  true, Tag = 1 , HasDefaultValue =  false )  ]
    
        public ProgramInvocationState ProgramInvocationState
        {
            get { return programInvocationState_; }
            set { programInvocationState_ = value;  }
        }
        
                
  

            public void initWithDefaults() {
            	
            }


            private static IASN1PreparedElementData preparedData = CoderFactory.getInstance().newPreparedElementData(typeof(StopUnitControl_Error));
            public IASN1PreparedElementData PreparedData {
            	get { return preparedData; }
            }

            
    }
            
}
