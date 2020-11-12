
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
    [ASN1Choice ( Name = "DeleteEventAction_Request") ]
    public class DeleteEventAction_Request : IASN1PreparedElement {
                    
        
	private System.Collections.Generic.ICollection<ObjectName> specific_ ;
        private bool  specific_selected = false ;
        
                
        
[ASN1SequenceOf( Name = "specific", IsSetOf = false  )]

    
        [ASN1Element ( Name = "specific", IsOptional =  false , HasTag =  true, Tag = 0 , HasDefaultValue =  false )  ]
    
        public System.Collections.Generic.ICollection<ObjectName> Specific
        {
            get { return specific_; }
            set { selectSpecific(value); }
        }
        
                
          
        
	private NullObject aa_specific_ ;
        private bool  aa_specific_selected = false ;
        
                
        
        [ASN1Null ( Name = "aa-specific" )]
    
        [ASN1Element ( Name = "aa-specific", IsOptional =  false , HasTag =  true, Tag = 1 , HasDefaultValue =  false )  ]
    
        public NullObject Aa_specific
        {
            get { return aa_specific_; }
            set { selectAa_specific(value); }
        }
        
                
          
        
	private Identifier domain_ ;
        private bool  domain_selected = false ;
        
                
        
        [ASN1Element ( Name = "domain", IsOptional =  false , HasTag =  true, Tag = 3 , HasDefaultValue =  false )  ]
    
        public Identifier Domain
        {
            get { return domain_; }
            set { selectDomain(value); }
        }
        
                
          
        
	private NullObject vmd_ ;
        private bool  vmd_selected = false ;
        
                
        
        [ASN1Null ( Name = "vmd" )]
    
        [ASN1Element ( Name = "vmd", IsOptional =  false , HasTag =  true, Tag = 4 , HasDefaultValue =  false )  ]
    
        public NullObject Vmd
        {
            get { return vmd_; }
            set { selectVmd(value); }
        }
        
                
          
        public bool isSpecificSelected () {
            return this.specific_selected ;
        }

        


        public void selectSpecific (System.Collections.Generic.ICollection<ObjectName> val) {
            this.specific_ = val;
            this.specific_selected = true;
            
            
                    this.aa_specific_selected = false;
                
                    this.domain_selected = false;
                
                    this.vmd_selected = false;
                            
        }
        
          
        public bool isAa_specificSelected () {
            return this.aa_specific_selected ;
        }

        
        public void selectAa_specific () {
            selectAa_specific (new NullObject());
	}
	


        public void selectAa_specific (NullObject val) {
            this.aa_specific_ = val;
            this.aa_specific_selected = true;
            
            
                    this.specific_selected = false;
                
                    this.aa_specific_selected = false;
                
                    this.domain_selected = false;
                
                    this.vmd_selected = false;
                            
        }
        
          
        public bool isDomainSelected () {
            return this.domain_selected ;
        }

        


        public void selectDomain (Identifier val) {
            this.domain_ = val;
            this.domain_selected = true;
            
            
                    this.specific_selected = false;
                
                    this.aa_specific_selected = false;
                
                    this.vmd_selected = false;
                            
        }
        
          
        public bool isVmdSelected () {
            return this.vmd_selected ;
        }

        
        public void selectVmd () {
            selectVmd (new NullObject());
	}
	


        public void selectVmd (NullObject val) {
            this.vmd_ = val;
            this.vmd_selected = true;
            
            
                    this.specific_selected = false;
                
                    this.aa_specific_selected = false;
                
                    this.domain_selected = false;
                            
        }
        
  

            public void initWithDefaults()
	    {
	    }

            private static IASN1PreparedElementData preparedData = CoderFactory.getInstance().newPreparedElementData(typeof(DeleteEventAction_Request));
            public IASN1PreparedElementData PreparedData {
            	get { return preparedData; }
            }

    }
            
}