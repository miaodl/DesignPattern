using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace _20300InterpreterPatternComplier
{
    class Program
    {
        static void Main(string[] args)
        {

            TestClass tc = new TestClass();
            tc.Test();

        }
    }

    //第十七章
    //解释器模式

    //Given a language, define a represention for its grammer along with an interperter that uses the representation to interpret sentences in the language

    /*
     * 编译原理 编译过程
     * 
     * Source
     *    |
     *    词法分析:逐个字符读入识别出各个单词
     *    |
     *    语法分析：将单词序列分解成各类语法短语
     *    |
     *    语义分析：审查源码是否有语义错误
     *    |
     *    中间代码生成：将源码变成一种内部表示
     *    |
     *    代码优化：对中间代码进行改善
     *    |
     *    目标代码生成：把优化后的中间代码变成可执行的内容
     *    |
     * Executable
     *    
     * 可充分利用.NET平台 正则表达式 XML
     */

    /*
     * ***********************************************************
     * 用解释器实现业务语言与运行环境间的映射
     * 
     * 
     *                                  |应用定制对象系统
     *                                  |------------------------->CommonLibrary
     *                                  |
     *                                  |
     * 定制业务语言--------->interpreter
     * 表达式/程序/脚本                 |                
     *                                  |直接翻译成.NET语言或IL
     *                                  |------------------------->.NET CLR
     *                                  |                                 
     * 
     */


    /*
     * 17.2
     * 解释器模式适用于哪些环境
     * 1，虽然相关操作频繁出现，而且有一定规律可循，但如果通过大量层次性的类表示这种操作，设计上显得比较复杂
     * 
     * 2，执行上效率的要求不是特别高，但对于灵活性的要求非常高
     */


    //page 318 355实例

    /*
     * 
     * Context<--------Client-------->interface
     *                                IExpression-----------------------------|
     *                                +Interpret(Context):void                |                      
     *                              /\                 /\                     |               
     *                             /                     \                    |             
     *                            /                       \                   |            
     *                           /                         \                  <>             
     *          TerminalExpression                          NonTerminalExpression   
     *           +Interpret(Context):void                   +Interpret(Context):void                  
     *                                          
     *                                          
     */

    //支持个位整数加减法小计算器程序，如何解释经典的解释器模式

    //C#抽象的表达式语言对象及Context对象
    //用于保存计算过程的中间结果及当前执行的操作符
    public class Context
    {
        public int Value;
        public char Operator;
    }

    //表示所有表达式的抽象接口
    public interface IExpression
    {
        //用Context负责保存中间结果
        void Evaluate(Context context);
    }

    //C#才分后的表达式元素，包括操作符和操作数
    //表示所有操作符
    public class Operator:IExpression
    {
        private char op;
        public Operator(char op)
        {
            this.op = op;
        }


        public virtual void Evaluate(Context context)
        {
            context.Operator = op;
        }
    }

    //表示所有操作数
    public class Operand:IExpression
    {
        int num;
        public Operand(int num)
        {
            this.num = num;
        }

        //根据操作符执行计算
        public virtual void Evaluate(Context c)
        {
            switch(c.Operator)
            {
                case '\0': c.Value = num; break;
                case '+': c.Value += num; break;
                case '-': c.Value -= num; break;
            }
        }
    }

    //解析器
    public class Calculator
    {
        public int Calculate(string expression)
        {
            Context context = new Context();
            IList<IExpression> tree = new List<IExpression>();

            //词法和语法分析
            char[] elements = expression.ToCharArray();
            foreach(char c in elements)
            {
                if ((c == '+') || (c == '-'))
                    tree.Add(new Operator(c));
                else
                    tree.Add(new Operand((int)(c-48)));
            }

            //便利中间每个过程
            foreach(IExpression exp in tree)
                exp.Evaluate(context);
            return context.Value;
        }
        
    }

    public class TestClass
    {
        public void Test()
        {
            Calculator calculator = new Calculator();
            Console.WriteLine(calculator.Calculate("1+5+1"));

            Console.ReadLine();
        }

    }


    //17.3采用正则表达式

    /*
     * 正则表达式解析器模式
     * 
     *   interface                                 interface                  
     *   IExpression                               IRegExpression                         
     *  +Interpret(Context):void<------------------+Expression:strng                     
     *                                             +interpret(Context):void                   
     *                                                     /|\                 
     *                                                      |                                  
     *                                                      |                             
     *    System.Text.RegularExpressions           RegExpressionBase
     *               Regex   <------------------+Interpret(Context):void
     *                                                     /|\                             
     *                                                      |                             
     *                                                      |                             
     *                                            ConcreteRegExpression                               
     *                                           +Interpret(Contex):void                   
     *                                                                                   
     *                                                                                   
     */


    //C#定义Context和表达式解析接口Page323 360
    //用于保存计算过程的中间结果及当前执行的操作符
    //根据Regex的功能操作符包括:Matches(M)、Replace(R)

    public interface IExpression2
    {
        void Interpret(Context context);
    }

    public class Context2
    {
        //文本内容
        public string Content2;

        //M matches /R replace
        public char Operator2;
        //匹配字符串合集
        public IList<string> Matches=new List<string>();
        //用于替换的文本内容
        public string Replacement;
    }

    //采用正则表达式方式表示的抽象接口
    public interface IRegExpression:IExpression2
    {
        //是否匹配
        bool IsMatch(string content);
    }

    //为减轻每个具体正则表达式解析类型的负担而增加的抽象基类
    //采用正则表达式表示的抽象基类
    public abstract class RegExpressionBase:IRegExpression
    {
        protected Regex regex;
        public RegExpressionBase(string expression)
        {
            regex = new Regex(expression, RegexOptions.Compiled | RegexOptions.IgnoreCase);
        }

        public virtual bool IsMatch(string content)
        {
            return regex.IsMatch(content);
        }

        //解析表达式
        public virtual void Evaluate(Context2 context)
        {
            if (context == null) throw new ArgumentNullException("context");
            switch(context.Operator2)
            {
                case 'M':
                    EvaluateMatch(context);
                    break;
                case 'R':
                    EvaluateReplace(context);
                    break;
                default:throw new ArgumentException();
            }
        }
    }
    //.................


    //17.4采用字典page 324 361
    //后面没看完。。。。

}
