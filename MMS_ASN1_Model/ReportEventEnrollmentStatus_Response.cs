
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
    [ASN1Sequence ( Name = "ReportEventEnrollmentStatus_Response", IsSet = false  )]
    public class ReportEventEnrollmentStatus_Response : IASN1PreparedElement {
                    
	private Transitions eventConditionTransitions_ ;
	
        [ASN1ElementAtr ( Name = "eventConditionTransitions", IsOptional =  false , HasTag =  true, Tag = 0 , HasDefaultValue =  false )  ]
    
        public Transitions EventConditionTransitions
        {
            get { return eventConditionTransitions_; }
            set { eventConditionTransitions_ = value;  }
        }
        
                
          
	private bool notificationLost_ ;
	[ASN1Boolean( Name = "" )]
    
        [ASN1ElementAtr ( Name = "notificationLost", IsOptional =  false , HasTag =  true, Tag = 1 , HasDefaultValue =  true )  ]
    
        public bool NotificationLost
        {
            get { return notificationLost_; }
            set { notificationLost_ = value;  }
        }
        
                
          
	private EE_Duration duration_ ;
	
        [ASN1ElementAtr ( Name = "duration", IsOptional =  false , HasTag =  true, Tag = 2 , HasDefaultValue =  false )  ]
    
        public EE_Duration Duration
        {
            get { return duration_; }
            set { duration_ = value;  }
        }
        
                
          
	private AlarmAckRule alarmAcknowledgmentRule_ ;
	
        private bool  alarmAcknowledgmentRule_present = false ;
	
        [ASN1ElementAtr ( Name = "alarmAcknowledgmentRule", IsOptional =  true , HasTag =  true, Tag = 3 , HasDefaultValue =  false )  ]
    
        public AlarmAckRule AlarmAcknowledgmentRule
        {
            get { return alarmAcknowledgmentRule_; }
            set { alarmAcknowledgmentRule_ = value; alarmAcknowledgmentRule_present = true;  }
        }
        
                
          
	private EE_State currentState_ ;
	
        [ASN1ElementAtr ( Name = "currentState", IsOptional =  false , HasTag =  true, Tag = 4 , HasDefaultValue =  false )  ]
    
        public EE_State CurrentState
        {
            get { return currentState_; }
            set { currentState_ = value;  }
        }
        
                
  
        public bool isAlarmAcknowledgmentRulePresent () {
            return this.alarmAcknowledgmentRule_present == true;
        }
        

            public void initWithDefaults() {
            	bool param_NotificationLost =
            false;
        NotificationLost = param_NotificationLost;
    
            }


            private static IASN1PreparedElementData preparedData = CoderFactory.getInstance().newPreparedElementData(typeof(ReportEventEnrollmentStatus_Response));
            public IASN1PreparedElementData PreparedData {
            	get { return preparedData; }
            }

            
    }
            
}
