
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
    [ASN1Sequence ( Name = "Output_Request", IsSet = false  )]
    public class Output_Request : IASN1PreparedElement {
                    
	private Identifier operatorStationName_ ;
	
        [ASN1Element ( Name = "operatorStationName", IsOptional =  false , HasTag =  true, Tag = 0 , HasDefaultValue =  false )  ]
    
        public Identifier OperatorStationName
        {
            get { return operatorStationName_; }
            set { operatorStationName_ = value;  }
        }
        
                
          
	private System.Collections.Generic.ICollection<MMSString> listOfOutputData_ ;
	
[ASN1SequenceOf( Name = "listOfOutputData", IsSetOf = false  )]

    
        [ASN1Element ( Name = "listOfOutputData", IsOptional =  false , HasTag =  true, Tag = 1 , HasDefaultValue =  false )  ]
    
        public System.Collections.Generic.ICollection<MMSString> ListOfOutputData
        {
            get { return listOfOutputData_; }
            set { listOfOutputData_ = value;  }
        }
        
                
  

            public void initWithDefaults() {
            	
            }


            private static IASN1PreparedElementData preparedData = CoderFactory.getInstance().newPreparedElementData(typeof(Output_Request));
            public IASN1PreparedElementData PreparedData {
            	get { return preparedData; }
            }

            
    }
            
}
