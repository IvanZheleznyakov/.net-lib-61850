
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
    [ASN1Sequence ( Name = "Event_Action_instance", IsSet = false  )]
    public class Event_Action_instance : IASN1PreparedElement {
                    
	private ObjectName name_ ;
	
        [ASN1ElementAtr ( Name = "name", IsOptional =  false , HasTag =  true, Tag = 0 , HasDefaultValue =  false )  ]
    
        public ObjectName Name
        {
            get { return name_; }
            set { name_ = value;  }
        }
        
                
          
	private DefinitionChoiceType definition_ ;
	

    [ASN1PreparedElement]    
    [ASN1Choice ( Name = "definition" )]
    public class DefinitionChoiceType : IASN1PreparedElement  {
	            
        
	private ObjectIdentifier reference_ ;
        private bool  reference_selected = false ;
        
                
        [ASN1ObjectIdentifier( Name = "" )]
    
        [ASN1ElementAtr ( Name = "reference", IsOptional =  false , HasTag =  true, Tag = 1 , HasDefaultValue =  false )  ]
    
        public ObjectIdentifier Reference
        {
            get { return reference_; }
            set { selectReference(value); }
        }
        
                
          
        
	private DetailsSequenceType details_ ;
        private bool  details_selected = false ;
        
                
        
       [ASN1PreparedElement]
       [ASN1Sequence ( Name = "details", IsSet = false  )]
       public class DetailsSequenceType : IASN1PreparedElement {
                        
	private Access_Control_List_instance accessControl_ ;
	
        [ASN1ElementAtr ( Name = "accessControl", IsOptional =  false , HasTag =  true, Tag = 3 , HasDefaultValue =  false )  ]
    
        public Access_Control_List_instance AccessControl
        {
            get { return accessControl_; }
            set { accessControl_ = value;  }
        }
        
                
          
	private ConfirmedServiceRequest confirmedServiceRequest_ ;
	
        [ASN1ElementAtr ( Name = "confirmedServiceRequest", IsOptional =  false , HasTag =  true, Tag = 4 , HasDefaultValue =  false )  ]
    
        public ConfirmedServiceRequest ConfirmedServiceRequest
        {
            get { return confirmedServiceRequest_; }
            set { confirmedServiceRequest_ = value;  }
        }
        
                
          
	private System.Collections.Generic.ICollection<Modifier> modifiers_ ;
	
[ASN1SequenceOf( Name = "modifiers", IsSetOf = false  )]

    
        [ASN1ElementAtr ( Name = "modifiers", IsOptional =  false , HasTag =  true, Tag = 5 , HasDefaultValue =  false )  ]
    
        public System.Collections.Generic.ICollection<Modifier> Modifiers
        {
            get { return modifiers_; }
            set { modifiers_ = value;  }
        }
        
                
          
	private System.Collections.Generic.ICollection<Event_Enrollment_instance> eventEnrollments_ ;
	
[ASN1SequenceOf( Name = "eventEnrollments", IsSetOf = false  )]

    
        [ASN1ElementAtr ( Name = "eventEnrollments", IsOptional =  false , HasTag =  true, Tag = 6 , HasDefaultValue =  false )  ]
    
        public System.Collections.Generic.ICollection<Event_Enrollment_instance> EventEnrollments
        {
            get { return eventEnrollments_; }
            set { eventEnrollments_ = value;  }
        }
        
                
  
                
                public void initWithDefaults() {
            		
                }

            private static IASN1PreparedElementData preparedData = CoderFactory.getInstance().newPreparedElementData(typeof(DetailsSequenceType));
            public IASN1PreparedElementData PreparedData {
            	get { return preparedData; }
            }

                
       }
                
        [ASN1ElementAtr ( Name = "details", IsOptional =  false , HasTag =  true, Tag = 2 , HasDefaultValue =  false )  ]
    
        public DetailsSequenceType Details
        {
            get { return details_; }
            set { selectDetails(value); }
        }
        
                
          
        public bool isReferenceSelected () {
            return this.reference_selected ;
        }

        


        public void selectReference (ObjectIdentifier val) {
            this.reference_ = val;
            this.reference_selected = true;
            
            
                    this.details_selected = false;
                            
        }
        
          
        public bool isDetailsSelected () {
            return this.details_selected ;
        }

        


        public void selectDetails (DetailsSequenceType val) {
            this.details_ = val;
            this.details_selected = true;
            
            
                    this.reference_selected = false;
                            
        }
        
  

            public void initWithDefaults()
	    {
	    }

            private static IASN1PreparedElementData preparedData = CoderFactory.getInstance().newPreparedElementData(typeof(DefinitionChoiceType));
            public IASN1PreparedElementData PreparedData {
            	get { return preparedData; }
            }

    }
                
        [ASN1ElementAtr ( Name = "definition", IsOptional =  false , HasTag =  false  , HasDefaultValue =  false )  ]
    
        public DefinitionChoiceType Definition
        {
            get { return definition_; }
            set { definition_ = value;  }
        }
        
                
  

            public void initWithDefaults() {
            	
            }


            private static IASN1PreparedElementData preparedData = CoderFactory.getInstance().newPreparedElementData(typeof(Event_Action_instance));
            public IASN1PreparedElementData PreparedData {
            	get { return preparedData; }
            }

            
    }
            
}
