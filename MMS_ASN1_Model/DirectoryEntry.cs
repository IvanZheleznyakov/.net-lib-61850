
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
    [ASN1Sequence ( Name = "DirectoryEntry", IsSet = false  )]
    public class DirectoryEntry : IASN1PreparedElement {
                    
	private FileName fileName_ ;

    [ASN1ElementAtr(Name = "fileName", IsOptional = false, HasTag = true, Tag = 0, HasDefaultValue = false, IsImplicitTag = true)]
        public FileName FileName
        {
            get { return fileName_; }
            set { fileName_ = value;  }
        }
        
                
          
	private FileAttributes fileAttributes_ ;
	
        [ASN1ElementAtr ( Name = "fileAttributes", IsOptional =  false , HasTag =  true, Tag = 1 , HasDefaultValue =  false )  ]
    
        public FileAttributes FileAttributes
        {
            get { return fileAttributes_; }
            set { fileAttributes_ = value;  }
        }
        
                
  

            public void initWithDefaults() {
            	
            }


            private static IASN1PreparedElementData preparedData = CoderFactory.getInstance().newPreparedElementData(typeof(DirectoryEntry));
            public IASN1PreparedElementData PreparedData {
            	get { return preparedData; }
            }

            
    }
            
}
