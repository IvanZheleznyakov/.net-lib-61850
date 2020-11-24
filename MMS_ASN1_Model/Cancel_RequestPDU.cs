
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
    [ASN1BoxedType ( Name = "Cancel_RequestPDU") ]
    public class Cancel_RequestPDU: IASN1PreparedElement {
            
           
        private Unsigned32  val;

        
        [ASN1ElementAtr ( Name = "Cancel-RequestPDU", IsOptional =  false , HasTag =  false  , HasDefaultValue =  false )  ]
    
        public Unsigned32 Value
        {
                get { return val; }        
                    
                set { val = value; }
                        
        }            

                    
        
        public Cancel_RequestPDU ()
        {
        }

            public void initWithDefaults()
	    {
	    }

        // STATICHERE
            private static IASN1PreparedElementData preparedData = CoderFactory.getInstance().newPreparedElementData(typeof(Cancel_RequestPDU));
            public IASN1PreparedElementData PreparedData {
            	get { return preparedData; }
            }

        
    }
            
}
