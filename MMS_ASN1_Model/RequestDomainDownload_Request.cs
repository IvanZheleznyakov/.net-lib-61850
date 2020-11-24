
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
    [ASN1Sequence ( Name = "RequestDomainDownload_Request", IsSet = false  )]
    public class RequestDomainDownload_Request : IASN1PreparedElement {
                    
	private Identifier domainName_ ;
	
        [ASN1ElementAtr ( Name = "domainName", IsOptional =  false , HasTag =  true, Tag = 0 , HasDefaultValue =  false )  ]
    
        public Identifier DomainName
        {
            get { return domainName_; }
            set { domainName_ = value;  }
        }
        
                
          
	private System.Collections.Generic.ICollection<MMSString> listOfCapabilities_ ;
	
        private bool  listOfCapabilities_present = false ;
	
[ASN1SequenceOf( Name = "listOfCapabilities", IsSetOf = false  )]

    
        [ASN1ElementAtr ( Name = "listOfCapabilities", IsOptional =  true , HasTag =  true, Tag = 1 , HasDefaultValue =  false )  ]
    
        public System.Collections.Generic.ICollection<MMSString> ListOfCapabilities
        {
            get { return listOfCapabilities_; }
            set { listOfCapabilities_ = value; listOfCapabilities_present = true;  }
        }
        
                
          
	private bool sharable_ ;
	[ASN1Boolean( Name = "" )]
    
        [ASN1ElementAtr ( Name = "sharable", IsOptional =  false , HasTag =  true, Tag = 2 , HasDefaultValue =  false )  ]
    
        public bool Sharable
        {
            get { return sharable_; }
            set { sharable_ = value;  }
        }
        
                
          
	private FileName fileName_ ;
	
        [ASN1ElementAtr ( Name = "fileName", IsOptional =  false , HasTag =  true, Tag = 4 , HasDefaultValue =  false )  ]
    
        public FileName FileName
        {
            get { return fileName_; }
            set { fileName_ = value;  }
        }
        
                
  
        public bool isListOfCapabilitiesPresent () {
            return this.listOfCapabilities_present == true;
        }
        

            public void initWithDefaults() {
            	
            }


            private static IASN1PreparedElementData preparedData = CoderFactory.getInstance().newPreparedElementData(typeof(RequestDomainDownload_Request));
            public IASN1PreparedElementData PreparedData {
            	get { return preparedData; }
            }

            
    }
            
}
