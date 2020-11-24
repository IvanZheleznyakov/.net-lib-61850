
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
    [ASN1Sequence ( Name = "Variable_List_Item_instance", IsSet = false  )]
    public class Variable_List_Item_instance : IASN1PreparedElement {
                    
	private Unnamed_Variable_instance unnamedItem_ ;
	
        private bool  unnamedItem_present = false ;
	
        [ASN1ElementAtr ( Name = "unnamedItem", IsOptional =  true , HasTag =  true, Tag = 0 , HasDefaultValue =  false )  ]
    
        public Unnamed_Variable_instance UnnamedItem
        {
            get { return unnamedItem_; }
            set { unnamedItem_ = value; unnamedItem_present = true;  }
        }
        
                
          
	private Named_Variable_instance namedItem_ ;
	
        private bool  namedItem_present = false ;
	
        [ASN1ElementAtr ( Name = "namedItem", IsOptional =  true , HasTag =  true, Tag = 1 , HasDefaultValue =  false )  ]
    
        public Named_Variable_instance NamedItem
        {
            get { return namedItem_; }
            set { namedItem_ = value; namedItem_present = true;  }
        }
        
                
          
	private AlternateAccess alternateAccess_ ;
	
        private bool  alternateAccess_present = false ;
	
        [ASN1ElementAtr ( Name = "alternateAccess", IsOptional =  true , HasTag =  true, Tag = 2 , HasDefaultValue =  false )  ]
    
        public AlternateAccess AlternateAccess
        {
            get { return alternateAccess_; }
            set { alternateAccess_ = value; alternateAccess_present = true;  }
        }
        
                
  
        public bool isUnnamedItemPresent () {
            return this.unnamedItem_present == true;
        }
        
        public bool isNamedItemPresent () {
            return this.namedItem_present == true;
        }
        
        public bool isAlternateAccessPresent () {
            return this.alternateAccess_present == true;
        }
        

            public void initWithDefaults() {
            	
            }


            private static IASN1PreparedElementData preparedData = CoderFactory.getInstance().newPreparedElementData(typeof(Variable_List_Item_instance));
            public IASN1PreparedElementData PreparedData {
            	get { return preparedData; }
            }

            
    }
            
}
