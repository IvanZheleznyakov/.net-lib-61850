
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
    [ASN1Sequence ( Name = "GetDataExchangeAttributes_Response", IsSet = false  )]
    public class GetDataExchangeAttributes_Response : IASN1PreparedElement {
                    
	private bool inUse_ ;
	[ASN1Boolean( Name = "" )]
    
        [ASN1Element ( Name = "inUse", IsOptional =  false , HasTag =  true, Tag = 0 , HasDefaultValue =  false )  ]
    
        public bool InUse
        {
            get { return inUse_; }
            set { inUse_ = value;  }
        }
        
                
          
	private System.Collections.Generic.ICollection<TypeDescription> listOfRequestTypeDescriptions_ ;
	
[ASN1SequenceOf( Name = "listOfRequestTypeDescriptions", IsSetOf = false  )]

    
        [ASN1Element ( Name = "listOfRequestTypeDescriptions", IsOptional =  false , HasTag =  true, Tag = 1 , HasDefaultValue =  false )  ]
    
        public System.Collections.Generic.ICollection<TypeDescription> ListOfRequestTypeDescriptions
        {
            get { return listOfRequestTypeDescriptions_; }
            set { listOfRequestTypeDescriptions_ = value;  }
        }
        
                
          
	private System.Collections.Generic.ICollection<TypeDescription> listOfResponseTypeDescriptions_ ;
	
[ASN1SequenceOf( Name = "listOfResponseTypeDescriptions", IsSetOf = false  )]

    
        [ASN1Element ( Name = "listOfResponseTypeDescriptions", IsOptional =  false , HasTag =  true, Tag = 2 , HasDefaultValue =  false )  ]
    
        public System.Collections.Generic.ICollection<TypeDescription> ListOfResponseTypeDescriptions
        {
            get { return listOfResponseTypeDescriptions_; }
            set { listOfResponseTypeDescriptions_ = value;  }
        }
        
                
          
	private Identifier programInvocation_ ;
	
        private bool  programInvocation_present = false ;
	
        [ASN1Element ( Name = "programInvocation", IsOptional =  true , HasTag =  true, Tag = 3 , HasDefaultValue =  false )  ]
    
        public Identifier ProgramInvocation
        {
            get { return programInvocation_; }
            set { programInvocation_ = value; programInvocation_present = true;  }
        }
        
                
          
	private Identifier accessControlList_ ;
	
        private bool  accessControlList_present = false ;
	
        [ASN1Element ( Name = "accessControlList", IsOptional =  true , HasTag =  true, Tag = 4 , HasDefaultValue =  false )  ]
    
        public Identifier AccessControlList
        {
            get { return accessControlList_; }
            set { accessControlList_ = value; accessControlList_present = true;  }
        }
        
                
  
        public bool isProgramInvocationPresent () {
            return this.programInvocation_present == true;
        }
        
        public bool isAccessControlListPresent () {
            return this.accessControlList_present == true;
        }
        

            public void initWithDefaults() {
            	
            }


            private static IASN1PreparedElementData preparedData = CoderFactory.getInstance().newPreparedElementData(typeof(GetDataExchangeAttributes_Response));
            public IASN1PreparedElementData PreparedData {
            	get { return preparedData; }
            }

            
    }
            
}