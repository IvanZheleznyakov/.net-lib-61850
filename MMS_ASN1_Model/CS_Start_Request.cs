
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
    [ASN1BoxedType ( Name = "CS_Start_Request") ]
    public class CS_Start_Request: IASN1PreparedElement {
            
           
        private CS_Start_RequestChoiceType  val;

        

    [ASN1PreparedElement]    
    [ASN1Choice ( Name = "CS-Start-Request" )]
    public class CS_Start_RequestChoiceType : IASN1PreparedElement  {
	            
        
	private NullObject normal_ ;
        private bool  normal_selected = false ;
        
                
        
        [ASN1Null ( Name = "normal" )]
    
        [ASN1ElementAtr ( Name = "normal", IsOptional =  false , HasTag =  false  , HasDefaultValue =  false )  ]
    
        public NullObject Normal
        {
            get { return normal_; }
            set { selectNormal(value); }
        }
        
                
          
        
	private ControllingSequenceType controlling_ ;
        private bool  controlling_selected = false ;
        
                
        
       [ASN1PreparedElement]
       [ASN1Sequence ( Name = "controlling", IsSet = false  )]
       public class ControllingSequenceType : IASN1PreparedElement {
                        
	private string startLocation_ ;
	
        private bool  startLocation_present = false ;
	[ASN1String( Name = "", 
        StringType =  UniversalTags.VisibleString , IsUCS = false )]
        [ASN1ElementAtr ( Name = "startLocation", IsOptional =  true , HasTag =  true, Tag = 0 , HasDefaultValue =  false )  ]
    
        public string StartLocation
        {
            get { return startLocation_; }
            set { startLocation_ = value; startLocation_present = true;  }
        }
        
                
          
	private StartCount startCount_ ;
	
        [ASN1ElementAtr ( Name = "startCount", IsOptional =  false , HasTag =  true, Tag = 1 , HasDefaultValue =  true )  ]
    
        public StartCount StartCount
        {
            get { return startCount_; }
            set { startCount_ = value;  }
        }
        
                
  
        public bool isStartLocationPresent () {
            return this.startLocation_present == true;
        }
        
                
                public void initWithDefaults() {
            		StartCount param_StartCount =         
            null;
        StartCount = param_StartCount;
    
                }

            private static IASN1PreparedElementData preparedData = CoderFactory.getInstance().newPreparedElementData(typeof(ControllingSequenceType));
            public IASN1PreparedElementData PreparedData {
            	get { return preparedData; }
            }

                
       }
                
        [ASN1ElementAtr ( Name = "controlling", IsOptional =  false , HasTag =  false  , HasDefaultValue =  false )  ]
    
        public ControllingSequenceType Controlling
        {
            get { return controlling_; }
            set { selectControlling(value); }
        }
        
                
          
        public bool isNormalSelected () {
            return this.normal_selected ;
        }

        
        public void selectNormal () {
            selectNormal (new NullObject());
	}
	


        public void selectNormal (NullObject val) {
            this.normal_ = val;
            this.normal_selected = true;
            
            
                    this.controlling_selected = false;
                            
        }
        
          
        public bool isControllingSelected () {
            return this.controlling_selected ;
        }

        


        public void selectControlling (ControllingSequenceType val) {
            this.controlling_ = val;
            this.controlling_selected = true;
            
            
                    this.normal_selected = false;
                            
        }
        
  

            public void initWithDefaults()
	    {
	    }

            private static IASN1PreparedElementData preparedData = CoderFactory.getInstance().newPreparedElementData(typeof(CS_Start_RequestChoiceType));
            public IASN1PreparedElementData PreparedData {
            	get { return preparedData; }
            }

    }
                
        [ASN1ElementAtr ( Name = "CS-Start-Request", IsOptional =  false , HasTag =  true, Tag = 0 , HasDefaultValue =  false )  ]
    
        public CS_Start_RequestChoiceType Value
        {
                get { return val; }        
                    
                set { val = value; }
                        
        }            

                    
        
        public CS_Start_Request ()
        {
        }

            public void initWithDefaults()
	    {
	    }


            private static IASN1PreparedElementData preparedData = CoderFactory.getInstance().newPreparedElementData(typeof(CS_Start_Request));
            public IASN1PreparedElementData PreparedData {
            	get { return preparedData; }
            }

        
    }
            
}
