
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
    [ASN1Sequence ( Name = "GetNameList_Request", IsSet = false  )]
    public class GetNameList_Request : IASN1PreparedElement {
                    
	private ObjectClass objectClass_ ;
	
        [ASN1Element ( Name = "objectClass", IsOptional =  false , HasTag =  true, Tag = 0 , HasDefaultValue =  false )  ]
    
        public ObjectClass ObjectClass
        {
            get { return objectClass_; }
            set { objectClass_ = value;  }
        }
        
                
          
	private ObjectScopeChoiceType objectScope_ ;
	

    [ASN1PreparedElement]    
    [ASN1Choice ( Name = "objectScope" )]
    public class ObjectScopeChoiceType : IASN1PreparedElement  {
	            
        
	private NullObject vmdSpecific_ ;
        private bool  vmdSpecific_selected = false ;
        
                
        
        [ASN1Null ( Name = "vmdSpecific" )]
    
        [ASN1Element ( Name = "vmdSpecific", IsOptional =  false , HasTag =  true, Tag = 0 , HasDefaultValue =  false )  ]
    
        public NullObject VmdSpecific
        {
            get { return vmdSpecific_; }
            set { selectVmdSpecific(value); }
        }
        
                
          
        
	private Identifier domainSpecific_ ;
        private bool  domainSpecific_selected = false ;
        
                
        
        [ASN1Element ( Name = "domainSpecific", IsOptional =  false , HasTag =  true, Tag = 1 , HasDefaultValue =  false )  ]
    
        public Identifier DomainSpecific
        {
            get { return domainSpecific_; }
            set { selectDomainSpecific(value); }
        }
        
                
          
        
	private NullObject aaSpecific_ ;
        private bool  aaSpecific_selected = false ;
        
                
        
        [ASN1Null ( Name = "aaSpecific" )]
    
        [ASN1Element ( Name = "aaSpecific", IsOptional =  false , HasTag =  true, Tag = 2 , HasDefaultValue =  false )  ]
    
        public NullObject AaSpecific
        {
            get { return aaSpecific_; }
            set { selectAaSpecific(value); }
        }
        
                
          
        public bool isVmdSpecificSelected () {
            return this.vmdSpecific_selected ;
        }

        
        public void selectVmdSpecific () {
            selectVmdSpecific (new NullObject());
	}
	


        public void selectVmdSpecific (NullObject val) {
            this.vmdSpecific_ = val;
            this.vmdSpecific_selected = true;
            
            
                    this.domainSpecific_selected = false;
                
                    this.aaSpecific_selected = false;
                            
        }
        
          
        public bool isDomainSpecificSelected () {
            return this.domainSpecific_selected ;
        }

        


        public void selectDomainSpecific (Identifier val) {
            this.domainSpecific_ = val;
            this.domainSpecific_selected = true;
            
            
                    this.vmdSpecific_selected = false;
                
                    this.aaSpecific_selected = false;
                            
        }
        
          
        public bool isAaSpecificSelected () {
            return this.aaSpecific_selected ;
        }

        
        public void selectAaSpecific () {
            selectAaSpecific (new NullObject());
	}
	


        public void selectAaSpecific (NullObject val) {
            this.aaSpecific_ = val;
            this.aaSpecific_selected = true;
            
            
                    this.vmdSpecific_selected = false;
                
                    this.domainSpecific_selected = false;
                            
        }
        
  

            public void initWithDefaults()
	    {
	    }

            private static IASN1PreparedElementData preparedData = CoderFactory.getInstance().newPreparedElementData(typeof(ObjectScopeChoiceType));
            public IASN1PreparedElementData PreparedData {
            	get { return preparedData; }
            }

    }
                
        [ASN1Element ( Name = "objectScope", IsOptional =  false , HasTag =  true, Tag = 1 , HasDefaultValue =  false )  ]
    
        public ObjectScopeChoiceType ObjectScope
        {
            get { return objectScope_; }
            set { objectScope_ = value;  }
        }
        
                
          
	private Identifier continueAfter_ ;
	
        private bool  continueAfter_present = false ;
	
        [ASN1Element ( Name = "continueAfter", IsOptional =  true , HasTag =  true, Tag = 2 , HasDefaultValue =  false )  ]
    
        public Identifier ContinueAfter
        {
            get { return continueAfter_; }
            set { continueAfter_ = value; continueAfter_present = true;  }
        }
        
                
  
        public bool isContinueAfterPresent () {
            return this.continueAfter_present == true;
        }
        

            public void initWithDefaults() {
            	
            }


            private static IASN1PreparedElementData preparedData = CoderFactory.getInstance().newPreparedElementData(typeof(GetNameList_Request));
            public IASN1PreparedElementData PreparedData {
            	get { return preparedData; }
            }

            
    }
            
}
