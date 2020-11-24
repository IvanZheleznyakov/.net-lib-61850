
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
    [ASN1BoxedType ( Name = "DeleteUnitControl_Request") ]
    public class DeleteUnitControl_Request: IASN1PreparedElement {
            
           
        private Identifier  val;

        
        [ASN1ElementAtr ( Name = "DeleteUnitControl-Request", IsOptional =  false , HasTag =  false  , HasDefaultValue =  false )  ]
    
        public Identifier Value
        {
                get { return val; }        
                    
                set { val = value; }
                        
        }            

                    
        
        public DeleteUnitControl_Request ()
        {
        }

            public void initWithDefaults()
	    {
	    }


            private static IASN1PreparedElementData preparedData = CoderFactory.getInstance().newPreparedElementData(typeof(DeleteUnitControl_Request));
            public IASN1PreparedElementData PreparedData {
            	get { return preparedData; }
            }

        
    }
            
}
