/*
 Copyright 2006-2011 Abdulla Abdurakhmanov (abdulla@latestbit.com)
 
 Licensed under the Apache License, Version 2.0 (the "License");
 you may not use this file except in compliance with the License.
 You may obtain a copy of the License at

 http://www.apache.org/licenses/LICENSE-2.0

 Unless required by applicable law or agreed to in writing, software
 distributed under the License is distributed on an "AS IS" BASIS,
 WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 See the License for the specific language governing permissions and
 limitations under the License.
 */
using System;
using org.bn.coders;

namespace org.bn
{
	public class CoderFactory
	{
        private static readonly CoderFactory instance = new CoderFactory();

        public static CoderFactory getInstance() {
            return instance;
        }

        public IEncoder newEncoder() {
            return newEncoder("BER");
        }

        public IEncoder newEncoder(String encodingSchema) {
            if (encodingSchema.Equals("BER",StringComparison.CurrentCultureIgnoreCase))
            {
                return new org.bn.coders.ber.BEREncoder();
            }
            else if (encodingSchema.Equals("PER", StringComparison.CurrentCultureIgnoreCase) || 
                encodingSchema.Equals("PER/Aligned", StringComparison.CurrentCultureIgnoreCase) ||
                encodingSchema.Equals("PER/A", StringComparison.CurrentCultureIgnoreCase))
            {
                return new org.bn.coders.per.PERAlignedEncoder();
            }
            else if (encodingSchema.Equals("PER/Unaligned", StringComparison.CurrentCultureIgnoreCase)||
                encodingSchema.Equals("PER/U", StringComparison.CurrentCultureIgnoreCase))
            {
                return new org.bn.coders.per.PERUnalignedEncoder();
            }
            else if (encodingSchema.Equals("DER", StringComparison.CurrentCultureIgnoreCase))
            {
                return new org.bn.coders.der.DEREncoder();
            }
            else
            {
                throw new ArgumentException("Unknown encoding schema '"+encodingSchema+"'", "encodingSchema");
            }
        }

        public IDecoder newDecoder() {
            return newDecoder("BER");
        }

        public IDecoder newDecoder(String encodingSchema) {
            if (encodingSchema.Equals("BER", StringComparison.CurrentCultureIgnoreCase))
            {
                return new org.bn.coders.ber.BERDecoder();
            }
            else if (encodingSchema.Equals("PER", StringComparison.CurrentCultureIgnoreCase) || 
                encodingSchema.Equals("PER/Aligned", StringComparison.CurrentCultureIgnoreCase)||
                encodingSchema.Equals("PER/A", StringComparison.CurrentCultureIgnoreCase))
            {
                return new org.bn.coders.per.PERAlignedDecoder();
            }
            else if (encodingSchema.Equals("PER/Unaligned", StringComparison.CurrentCultureIgnoreCase)||
                encodingSchema.Equals("PER/U", StringComparison.CurrentCultureIgnoreCase))
            {
                return new org.bn.coders.per.PERUnalignedDecoder();
            }
            else if (encodingSchema.Equals("DER", StringComparison.CurrentCultureIgnoreCase))
            {
                return new org.bn.coders.der.DERDecoder();
            }
            else
            {
                throw new ArgumentException("Unknown encoding schema '" + encodingSchema + "'", "encodingSchema");
            }
        }

        public IASN1PreparedElementData newPreparedElementData(Type typeInfo)
        {
            return new ASN1PreparedElementData(typeInfo);
        }
	}
}