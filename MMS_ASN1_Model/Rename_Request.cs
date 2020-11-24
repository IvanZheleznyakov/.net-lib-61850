
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
    [ASN1Sequence ( Name = "Rename_Request", IsSet = false  )]
    public class Rename_Request : IASN1PreparedElement {
                    
	private ObjectClass objectClass_ ;
	
        [ASN1ElementAtr ( Name = "objectClass", IsOptional =  false , HasTag =  true, Tag = 0 , HasDefaultValue =  false )  ]
    
        public ObjectClass ObjectClass
        {
            get { return objectClass_; }
            set { objectClass_ = value;  }
        }
        
                
          
	private ObjectName currentName_ ;
	
        [ASN1ElementAtr ( Name = "currentName", IsOptional =  false , HasTag =  true, Tag = 1 , HasDefaultValue =  false )  ]
    
        public ObjectName CurrentName
        {
            get { return currentName_; }
            set { currentName_ = value;  }
        }
        
                
          
	private Identifier newIdentifier_ ;
	
        [ASN1ElementAtr ( Name = "newIdentifier", IsOptional =  false , HasTag =  true, Tag = 2 , HasDefaultValue =  false )  ]
    
        public Identifier NewIdentifier
        {
            get { return newIdentifier_; }
            set { newIdentifier_ = value;  }
        }
        
                
  

            public void initWithDefaults() {
            	
            }


            private static IASN1PreparedElementData preparedData = CoderFactory.getInstance().newPreparedElementData(typeof(Rename_Request));
            public IASN1PreparedElementData PreparedData {
            	get { return preparedData; }
            }

            
    }
            
}
