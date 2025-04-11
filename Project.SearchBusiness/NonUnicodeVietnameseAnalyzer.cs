using Lucene.Net.Analysis.Core;
using Lucene.Net.Analysis.Miscellaneous;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.Analysis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.SearchBusiness
{
    public class NonUnicodeVietnameseAnalyzer : Analyzer
    {
        private readonly Lucene.Net.Util.LuceneVersion _version;


        public NonUnicodeVietnameseAnalyzer(Lucene.Net.Util.LuceneVersion version)
        {
            _version = version;
        }

        protected override TokenStreamComponents CreateComponents(string fieldName, TextReader reader)
        {
            var tokenizer = new StandardTokenizer(_version, reader);

            TokenStream stream = new StandardFilter(_version, tokenizer);
            stream = new LowerCaseFilter(_version, stream);
            stream = new StopFilter(_version, stream, StopAnalyzer.ENGLISH_STOP_WORDS_SET);
            stream = new ASCIIFoldingFilter(stream);

            return new TokenStreamComponents(tokenizer, stream);
        }
    }
}
