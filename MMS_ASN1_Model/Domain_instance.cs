
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
    [ASN1Sequence ( Name = "Domain_instance", IsSet = false  )]
    public class Domain_instance : IASN1PreparedElement {
                    
	private Identifier name_ ;
	
        [ASN1ElementAtr ( Name = "name", IsOptional =  false , HasTag =  true, Tag = 0 , HasDefaultValue =  false )  ]
    
        public Identifier Name
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
                        
	private System.Collections.Generic.ICollection<MMSString> capabilities_ ;
	
[ASN1SequenceOf( Name = "capabilities", IsSetOf = false  )]

    
        [ASN1ElementAtr ( Name = "capabilities", IsOptional =  false , HasTag =  true, Tag = 3 , HasDefaultValue =  false )  ]
    
        public System.Collections.Generic.ICollection<MMSString> Capabilities
        {
            get { return capabilities_; }
            set { capabilities_ = value;  }
        }
        
                
          
	private DomainState state_ ;
	
        [ASN1ElementAtr ( Name = "state", IsOptional =  false , HasTag =  true, Tag = 4 , HasDefaultValue =  false )  ]
    
        public DomainState State
        {
            get { return state_; }
            set { state_ = value;  }
        }
        
                
          
	private Access_Control_List_instance accessControl_ ;
	
        [ASN1ElementAtr ( Name = "accessControl", IsOptional =  false , HasTag =  true, Tag = 5 , HasDefaultValue =  false )  ]
    
        public Access_Control_List_instance AccessControl
        {
            get { return accessControl_; }
            set { accessControl_ = value;  }
        }
        
                
          
	private bool sharable_ ;
	[ASN1Boolean( Name = "" )]
    
        [ASN1ElementAtr ( Name = "sharable", IsOptional =  false , HasTag =  true, Tag = 6 , HasDefaultValue =  false )  ]
    
        public bool Sharable
        {
            get { return sharable_; }
            set { sharable_ = value;  }
        }
        
                
          
	private System.Collections.Generic.ICollection<Program_Invocation_instance> programInvocations_ ;
	
[ASN1SequenceOf( Name = "programInvocations", IsSetOf = false  )]

    
        [ASN1ElementAtr ( Name = "programInvocations", IsOptional =  false , HasTag =  true, Tag = 7 , HasDefaultValue =  false )  ]
    
        public System.Collections.Generic.ICollection<Program_Invocation_instance> ProgramInvocations
        {
            get { return programInvocations_; }
            set { programInvocations_ = value;  }
        }
        
                
          
	private System.Collections.Generic.ICollection<Named_Variable_instance> namedVariables_ ;
	
[ASN1SequenceOf( Name = "namedVariables", IsSetOf = false  )]

    
        [ASN1ElementAtr ( Name = "namedVariables", IsOptional =  false , HasTag =  true, Tag = 8 , HasDefaultValue =  false )  ]
    
        public System.Collections.Generic.ICollection<Named_Variable_instance> NamedVariables
        {
            get { return namedVariables_; }
            set { namedVariables_ = value;  }
        }
        
                
          
	private System.Collections.Generic.ICollection<Named_Variable_List_instance> namedVariableLists_ ;
	
[ASN1SequenceOf( Name = "namedVariableLists", IsSetOf = false  )]

    
        [ASN1ElementAtr ( Name = "namedVariableLists", IsOptional =  false , HasTag =  true, Tag = 9 , HasDefaultValue =  false )  ]
    
        public System.Collections.Generic.ICollection<Named_Variable_List_instance> NamedVariableLists
        {
            get { return namedVariableLists_; }
            set { namedVariableLists_ = value;  }
        }
        
                
          
	private System.Collections.Generic.ICollection<Named_Type_instance> namedTypes_ ;
	
[ASN1SequenceOf( Name = "namedTypes", IsSetOf = false  )]

    
        [ASN1ElementAtr ( Name = "namedTypes", IsOptional =  false , HasTag =  true, Tag = 10 , HasDefaultValue =  false )  ]
    
        public System.Collections.Generic.ICollection<Named_Type_instance> NamedTypes
        {
            get { return namedTypes_; }
            set { namedTypes_ = value;  }
        }
        
                
          
	private System.Collections.Generic.ICollection<Event_Condition_instance> eventConditions_ ;
	
[ASN1SequenceOf( Name = "eventConditions", IsSetOf = false  )]

    
        [ASN1ElementAtr ( Name = "eventConditions", IsOptional =  false , HasTag =  true, Tag = 11 , HasDefaultValue =  false )  ]
    
        public System.Collections.Generic.ICollection<Event_Condition_instance> EventConditions
        {
            get { return eventConditions_; }
            set { eventConditions_ = value;  }
        }
        
                
          
	private System.Collections.Generic.ICollection<Event_Action_instance> eventActions_ ;
	
[ASN1SequenceOf( Name = "eventActions", IsSetOf = false  )]

    
        [ASN1ElementAtr ( Name = "eventActions", IsOptional =  false , HasTag =  true, Tag = 12 , HasDefaultValue =  false )  ]
    
        public System.Collections.Generic.ICollection<Event_Action_instance> EventActions
        {
            get { return eventActions_; }
            set { eventActions_ = value;  }
        }
        
                
          
	private System.Collections.Generic.ICollection<Event_Enrollment_instance> eventEnrollments_ ;
	
[ASN1SequenceOf( Name = "eventEnrollments", IsSetOf = false  )]

    
        [ASN1ElementAtr ( Name = "eventEnrollments", IsOptional =  false , HasTag =  true, Tag = 13 , HasDefaultValue =  false )  ]
    
        public System.Collections.Generic.ICollection<Event_Enrollment_instance> EventEnrollments
        {
            get { return eventEnrollments_; }
            set { eventEnrollments_ = value;  }
        }
        
                
          
	private System.Collections.Generic.ICollection<Event_Condition_List_instance> eventConditionLists_ ;
	
[ASN1SequenceOf( Name = "eventConditionLists", IsSetOf = false  )]

    
        [ASN1ElementAtr ( Name = "eventConditionLists", IsOptional =  false , HasTag =  true, Tag = 14 , HasDefaultValue =  false )  ]
    
        public System.Collections.Generic.ICollection<Event_Condition_List_instance> EventConditionLists
        {
            get { return eventConditionLists_; }
            set { eventConditionLists_ = value;  }
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


            private static IASN1PreparedElementData preparedData = CoderFactory.getInstance().newPreparedElementData(typeof(Domain_instance));
            public IASN1PreparedElementData PreparedData {
            	get { return preparedData; }
            }

            
    }
            
}
