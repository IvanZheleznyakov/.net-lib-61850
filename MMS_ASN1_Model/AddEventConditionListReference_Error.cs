
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
    [ASN1BoxedType ( Name = "AddEventConditionListReference_Error") ]
    public class AddEventConditionListReference_Error: IASN1PreparedElement {
            
           
        private ObjectName  val;

        
        [ASN1Element ( Name = "AddEventConditionListReference-Error", IsOptional =  false , HasTag =  false  , HasDefaultValue =  false )  ]
    
        public ObjectName Value
        {
                get { return val; }        
                    
                set { val = value; }
                        
        }            

                    
        
        public AddEventConditionListReference_Error ()
        {
        }

            public void initWithDefaults()
	    {
	    }


            private static IASN1PreparedElementData preparedData = CoderFactory.getInstance().newPreparedElementData(typeof(AddEventConditionListReference_Error));
            public IASN1PreparedElementData PreparedData {
            	get { return preparedData; }
            }

        
    }
            
}
