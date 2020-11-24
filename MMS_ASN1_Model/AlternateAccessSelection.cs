
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
    [ASN1Choice ( Name = "AlternateAccessSelection") ]
    public class AlternateAccessSelection : IASN1PreparedElement {
                    
        
	private SelectAlternateAccessSequenceType selectAlternateAccess_ ;
        private bool  selectAlternateAccess_selected = false ;
        
                
        
       [ASN1PreparedElement]
       [ASN1Sequence ( Name = "selectAlternateAccess", IsSet = false  )]
       public class SelectAlternateAccessSequenceType : IASN1PreparedElement {
                        
	private AccessSelectionChoiceType accessSelection_ ;
	

    [ASN1PreparedElement]    
    [ASN1Choice ( Name = "accessSelection" )]
    public class AccessSelectionChoiceType : IASN1PreparedElement  {
	            
        
	private Identifier component_ ;
        private bool  component_selected = false ;
        
                
        
        [ASN1ElementAtr ( Name = "component", IsOptional =  false , HasTag =  true, Tag = 0 , HasDefaultValue =  false )  ]
    
        public Identifier Component
        {
            get { return component_; }
            set { selectComponent(value); }
        }
        
                
          
        
	private Unsigned32 index_ ;
        private bool  index_selected = false ;
        
                
        
        [ASN1ElementAtr ( Name = "index", IsOptional =  false , HasTag =  true, Tag = 1 , HasDefaultValue =  false )  ]
    
        public Unsigned32 Index
        {
            get { return index_; }
            set { selectIndex(value); }
        }
        
                
          
        
	private IndexRangeSequenceType indexRange_ ;
        private bool  indexRange_selected = false ;
        
                
        
       [ASN1PreparedElement]
       [ASN1Sequence ( Name = "indexRange", IsSet = false  )]
       public class IndexRangeSequenceType : IASN1PreparedElement {
                        
	private Unsigned32 lowIndex_ ;
	
        [ASN1ElementAtr ( Name = "lowIndex", IsOptional =  false , HasTag =  true, Tag = 0 , HasDefaultValue =  false )  ]
    
        public Unsigned32 LowIndex
        {
            get { return lowIndex_; }
            set { lowIndex_ = value;  }
        }
        
                
          
	private Unsigned32 numberOfElements_ ;
	
        [ASN1ElementAtr ( Name = "numberOfElements", IsOptional =  false , HasTag =  true, Tag = 1 , HasDefaultValue =  false )  ]
    
        public Unsigned32 NumberOfElements
        {
            get { return numberOfElements_; }
            set { numberOfElements_ = value;  }
        }
        
                
  
                
                public void initWithDefaults() {
            		
                }

            private static IASN1PreparedElementData preparedData = CoderFactory.getInstance().newPreparedElementData(typeof(IndexRangeSequenceType));
            public IASN1PreparedElementData PreparedData {
            	get { return preparedData; }
            }

                
       }
                
        [ASN1ElementAtr ( Name = "indexRange", IsOptional =  false , HasTag =  true, Tag = 2 , HasDefaultValue =  false )  ]
    
        public IndexRangeSequenceType IndexRange
        {
            get { return indexRange_; }
            set { selectIndexRange(value); }
        }
        
                
          
        
	private NullObject allElements_ ;
        private bool  allElements_selected = false ;
        
                
        
        [ASN1Null ( Name = "allElements" )]
    
        [ASN1ElementAtr ( Name = "allElements", IsOptional =  false , HasTag =  true, Tag = 3 , HasDefaultValue =  false )  ]
    
        public NullObject AllElements
        {
            get { return allElements_; }
            set { selectAllElements(value); }
        }
        
                
          
        public bool isComponentSelected () {
            return this.component_selected ;
        }

        


        public void selectComponent (Identifier val) {
            this.component_ = val;
            this.component_selected = true;
            
            
                    this.index_selected = false;
                
                    this.indexRange_selected = false;
                
                    this.allElements_selected = false;
                            
        }
        
          
        public bool isIndexSelected () {
            return this.index_selected ;
        }

        


        public void selectIndex (Unsigned32 val) {
            this.index_ = val;
            this.index_selected = true;
            
            
                    this.component_selected = false;
                
                    this.indexRange_selected = false;
                
                    this.allElements_selected = false;
                            
        }
        
          
        public bool isIndexRangeSelected () {
            return this.indexRange_selected ;
        }

        


        public void selectIndexRange (IndexRangeSequenceType val) {
            this.indexRange_ = val;
            this.indexRange_selected = true;
            
            
                    this.component_selected = false;
                
                    this.index_selected = false;
                
                    this.allElements_selected = false;
                            
        }
        
          
        public bool isAllElementsSelected () {
            return this.allElements_selected ;
        }

        
        public void selectAllElements () {
            selectAllElements (new NullObject());
	}
	


        public void selectAllElements (NullObject val) {
            this.allElements_ = val;
            this.allElements_selected = true;
            
            
                    this.component_selected = false;
                
                    this.index_selected = false;
                
                    this.indexRange_selected = false;
                            
        }
        
  

            public void initWithDefaults()
	    {
	    }

            private static IASN1PreparedElementData preparedData = CoderFactory.getInstance().newPreparedElementData(typeof(AccessSelectionChoiceType));
            public IASN1PreparedElementData PreparedData {
            	get { return preparedData; }
            }

    }
                
        [ASN1ElementAtr ( Name = "accessSelection", IsOptional =  false , HasTag =  false  , HasDefaultValue =  false )  ]
    
        public AccessSelectionChoiceType AccessSelection
        {
            get { return accessSelection_; }
            set { accessSelection_ = value;  }
        }
        
                
          
	private AlternateAccess alternateAccess_ ;
	
        [ASN1ElementAtr ( Name = "alternateAccess", IsOptional =  false , HasTag =  false  , HasDefaultValue =  false )  ]
    
        public AlternateAccess AlternateAccess
        {
            get { return alternateAccess_; }
            set { alternateAccess_ = value;  }
        }
        
                
  
                
                public void initWithDefaults() {
            		
                }

            private static IASN1PreparedElementData preparedData = CoderFactory.getInstance().newPreparedElementData(typeof(SelectAlternateAccessSequenceType));
            public IASN1PreparedElementData PreparedData {
            	get { return preparedData; }
            }

                
       }
                
        [ASN1ElementAtr ( Name = "selectAlternateAccess", IsOptional =  false , HasTag =  true, Tag = 0 , HasDefaultValue =  false )  ]
    
        public SelectAlternateAccessSequenceType SelectAlternateAccess
        {
            get { return selectAlternateAccess_; }
            set { selectSelectAlternateAccess(value); }
        }
        
                
          
        
	private SelectAccessChoiceType selectAccess_ ;
        private bool  selectAccess_selected = false ;
        
                
        

    [ASN1PreparedElement]    
    [ASN1Choice ( Name = "selectAccess" )]
    public class SelectAccessChoiceType : IASN1PreparedElement  {
	            
        
	private Identifier component_ ;
        private bool  component_selected = false ;
        
                
        
        [ASN1ElementAtr ( Name = "component", IsOptional =  false , HasTag =  true, Tag = 1 , HasDefaultValue =  false )  ]
    
        public Identifier Component
        {
            get { return component_; }
            set { selectComponent(value); }
        }
        
                
          
        
	private Unsigned32 index_ ;
        private bool  index_selected = false ;
        
                
        
        [ASN1ElementAtr ( Name = "index", IsOptional =  false , HasTag =  true, Tag = 2 , HasDefaultValue =  false )  ]
    
        public Unsigned32 Index
        {
            get { return index_; }
            set { selectIndex(value); }
        }
        
                
          
        
	private IndexRangeSequenceType indexRange_ ;
        private bool  indexRange_selected = false ;
        
                
        
       [ASN1PreparedElement]
       [ASN1Sequence ( Name = "indexRange", IsSet = false  )]
       public class IndexRangeSequenceType : IASN1PreparedElement {
                        
	private Unsigned32 lowIndex_ ;
	
        [ASN1ElementAtr ( Name = "lowIndex", IsOptional =  false , HasTag =  true, Tag = 0 , HasDefaultValue =  false )  ]
    
        public Unsigned32 LowIndex
        {
            get { return lowIndex_; }
            set { lowIndex_ = value;  }
        }
        
                
          
	private Unsigned32 numberOfElements_ ;
	
        [ASN1ElementAtr ( Name = "numberOfElements", IsOptional =  false , HasTag =  true, Tag = 1 , HasDefaultValue =  false )  ]
    
        public Unsigned32 NumberOfElements
        {
            get { return numberOfElements_; }
            set { numberOfElements_ = value;  }
        }
        
                
  
                
                public void initWithDefaults() {
            		
                }

            private static IASN1PreparedElementData preparedData = CoderFactory.getInstance().newPreparedElementData(typeof(IndexRangeSequenceType));
            public IASN1PreparedElementData PreparedData {
            	get { return preparedData; }
            }

                
       }
                
        [ASN1ElementAtr ( Name = "indexRange", IsOptional =  false , HasTag =  true, Tag = 3 , HasDefaultValue =  false )  ]
    
        public IndexRangeSequenceType IndexRange
        {
            get { return indexRange_; }
            set { selectIndexRange(value); }
        }
        
                
          
        
	private NullObject allElements_ ;
        private bool  allElements_selected = false ;
        
                
        
        [ASN1Null ( Name = "allElements" )]
    
        [ASN1ElementAtr ( Name = "allElements", IsOptional =  false , HasTag =  true, Tag = 4 , HasDefaultValue =  false )  ]
    
        public NullObject AllElements
        {
            get { return allElements_; }
            set { selectAllElements(value); }
        }
        
                
          
        public bool isComponentSelected () {
            return this.component_selected ;
        }

        


        public void selectComponent (Identifier val) {
            this.component_ = val;
            this.component_selected = true;
            
            
                    this.index_selected = false;
                
                    this.indexRange_selected = false;
                
                    this.allElements_selected = false;
                            
        }
        
          
        public bool isIndexSelected () {
            return this.index_selected ;
        }

        


        public void selectIndex (Unsigned32 val) {
            this.index_ = val;
            this.index_selected = true;
            
            
                    this.component_selected = false;
                
                    this.indexRange_selected = false;
                
                    this.allElements_selected = false;
                            
        }
        
          
        public bool isIndexRangeSelected () {
            return this.indexRange_selected ;
        }

        


        public void selectIndexRange (IndexRangeSequenceType val) {
            this.indexRange_ = val;
            this.indexRange_selected = true;
            
            
                    this.component_selected = false;
                
                    this.index_selected = false;
                
                    this.allElements_selected = false;
                            
        }
        
          
        public bool isAllElementsSelected () {
            return this.allElements_selected ;
        }

        
        public void selectAllElements () {
            selectAllElements (new NullObject());
	}
	


        public void selectAllElements (NullObject val) {
            this.allElements_ = val;
            this.allElements_selected = true;
            
            
                    this.component_selected = false;
                
                    this.index_selected = false;
                
                    this.indexRange_selected = false;
                            
        }
        
  

            public void initWithDefaults()
	    {
	    }

            private static IASN1PreparedElementData preparedData = CoderFactory.getInstance().newPreparedElementData(typeof(SelectAccessChoiceType));
            public IASN1PreparedElementData PreparedData {
            	get { return preparedData; }
            }

    }
                
        [ASN1ElementAtr ( Name = "selectAccess", IsOptional =  false , HasTag =  false  , HasDefaultValue =  false )  ]
    
        public SelectAccessChoiceType SelectAccess
        {
            get { return selectAccess_; }
            set { selectSelectAccess(value); }
        }
        
                
          
        public bool isSelectAlternateAccessSelected () {
            return this.selectAlternateAccess_selected ;
        }

        


        public void selectSelectAlternateAccess (SelectAlternateAccessSequenceType val) {
            this.selectAlternateAccess_ = val;
            this.selectAlternateAccess_selected = true;
            
            
                    this.selectAccess_selected = false;
                            
        }
        
          
        public bool isSelectAccessSelected () {
            return this.selectAccess_selected ;
        }

        


        public void selectSelectAccess (SelectAccessChoiceType val) {
            this.selectAccess_ = val;
            this.selectAccess_selected = true;
            
            
                    this.selectAlternateAccess_selected = false;
                            
        }
        
  

            public void initWithDefaults()
	    {
	    }

            private static IASN1PreparedElementData preparedData = CoderFactory.getInstance().newPreparedElementData(typeof(AlternateAccessSelection));
            public IASN1PreparedElementData PreparedData {
            	get { return preparedData; }
            }

    }
            
}
