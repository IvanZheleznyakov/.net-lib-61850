
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
    [ASN1Sequence ( Name = "FileAttributes", IsSet = false  )]
    public class FileAttributes : IASN1PreparedElement {
                    
	private Unsigned32 sizeOfFile_ ;
	
        [ASN1ElementAtr ( Name = "sizeOfFile", IsOptional =  false , HasTag =  true, Tag = 0 , HasDefaultValue =  false )  ]
    
        public Unsigned32 SizeOfFile
        {
            get { return sizeOfFile_; }
            set { sizeOfFile_ = value;  }
        }
        
                
          
	private string lastModified_ ;
	
        private bool  lastModified_present = false ;
	[ASN1String( Name = "", 
        StringType = UniversalTags.GeneralizedTime , IsUCS = false )]
        [ASN1ElementAtr ( Name = "lastModified", IsOptional =  true , HasTag =  true, Tag = 1 , HasDefaultValue =  false )  ]
    
        public string LastModified
        {
            get { return lastModified_; }
            set { lastModified_ = value; lastModified_present = true;  }
        }
        
                
  
        public bool isLastModifiedPresent () {
            return this.lastModified_present == true;
        }
        

            public void initWithDefaults() {
            	
            }


            private static IASN1PreparedElementData preparedData = CoderFactory.getInstance().newPreparedElementData(typeof(FileAttributes));
            public IASN1PreparedElementData PreparedData {
            	get { return preparedData; }
            }

            
    }
            
}
