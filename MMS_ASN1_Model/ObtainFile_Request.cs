
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
    [ASN1Sequence ( Name = "ObtainFile_Request", IsSet = false  )]
    public class ObtainFile_Request : IASN1PreparedElement {
                    
	private ApplicationReference sourceFileServer_ ;
	
        private bool  sourceFileServer_present = false ;
	
        [ASN1ElementAtr ( Name = "sourceFileServer", IsOptional =  true , HasTag =  true, Tag = 0 , HasDefaultValue =  false )  ]
    
        public ApplicationReference SourceFileServer
        {
            get { return sourceFileServer_; }
            set { sourceFileServer_ = value; sourceFileServer_present = true;  }
        }
        
                
          
	private FileName sourceFile_ ;
	
        [ASN1ElementAtr ( Name = "sourceFile", IsOptional =  false , HasTag =  true, Tag = 1 , HasDefaultValue =  false )  ]
    
        public FileName SourceFile
        {
            get { return sourceFile_; }
            set { sourceFile_ = value;  }
        }
        
                
          
	private FileName destinationFile_ ;
	
        [ASN1ElementAtr ( Name = "destinationFile", IsOptional =  false , HasTag =  true, Tag = 2 , HasDefaultValue =  false )  ]
    
        public FileName DestinationFile
        {
            get { return destinationFile_; }
            set { destinationFile_ = value;  }
        }
        
                
  
        public bool isSourceFileServerPresent () {
            return this.sourceFileServer_present == true;
        }
        

            public void initWithDefaults() {
            	
            }


            private static IASN1PreparedElementData preparedData = CoderFactory.getInstance().newPreparedElementData(typeof(ObtainFile_Request));
            public IASN1PreparedElementData PreparedData {
            	get { return preparedData; }
            }

            
    }
            
}
