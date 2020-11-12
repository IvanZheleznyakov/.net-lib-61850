
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
    [ASN1Sequence ( Name = "FileManagementParameters", IsSet = false  )]
    public class FileManagementParameters : IASN1PreparedElement {
                    
	private MMSString fileName_ ;
	
        [ASN1Element ( Name = "fileName", IsOptional =  false , HasTag =  true, Tag = 0 , HasDefaultValue =  false )  ]
    
        public MMSString FileName
        {
            get { return fileName_; }
            set { fileName_ = value;  }
        }
        
                
  

            public void initWithDefaults() {
            	
            }


            private static IASN1PreparedElementData preparedData = CoderFactory.getInstance().newPreparedElementData(typeof(FileManagementParameters));
            public IASN1PreparedElementData PreparedData {
            	get { return preparedData; }
            }

            
    }
            
}