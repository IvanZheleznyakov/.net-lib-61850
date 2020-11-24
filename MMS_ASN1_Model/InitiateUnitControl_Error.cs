
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
    [ASN1Choice ( Name = "InitiateUnitControl_Error") ]
    public class InitiateUnitControl_Error : IASN1PreparedElement {
                    
        
	private Identifier domain_ ;
        private bool  domain_selected = false ;
        
                
        
        [ASN1ElementAtr ( Name = "domain", IsOptional =  false , HasTag =  true, Tag = 0 , HasDefaultValue =  false )  ]
    
        public Identifier Domain
        {
            get { return domain_; }
            set { selectDomain(value); }
        }
        
                
          
        
	private Identifier programInvocation_ ;
        private bool  programInvocation_selected = false ;
        
                
        
        [ASN1ElementAtr ( Name = "programInvocation", IsOptional =  false , HasTag =  true, Tag = 1 , HasDefaultValue =  false )  ]
    
        public Identifier ProgramInvocation
        {
            get { return programInvocation_; }
            set { selectProgramInvocation(value); }
        }
        
                
          
        public bool isDomainSelected () {
            return this.domain_selected ;
        }

        


        public void selectDomain (Identifier val) {
            this.domain_ = val;
            this.domain_selected = true;
            
            
                    this.programInvocation_selected = false;
                            
        }
        
          
        public bool isProgramInvocationSelected () {
            return this.programInvocation_selected ;
        }

        


        public void selectProgramInvocation (Identifier val) {
            this.programInvocation_ = val;
            this.programInvocation_selected = true;
            
            
                    this.domain_selected = false;
                            
        }
        
  

            public void initWithDefaults()
	    {
	    }

            private static IASN1PreparedElementData preparedData = CoderFactory.getInstance().newPreparedElementData(typeof(InitiateUnitControl_Error));
            public IASN1PreparedElementData PreparedData {
            	get { return preparedData; }
            }

    }
            
}
