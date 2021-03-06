using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;

namespace TeamDev.Redis.LanguageExtensions
{
  public class LinqParser
  {
    public void Parse(Expression expression)
    {
      // Reduce the linq Query to a simplier query. 
      while(expression.CanReduce)
        expression = expression.Reduce();

      switch (expression.NodeType)
      {
        case ExpressionType.Add:
          break;
        case ExpressionType.AddAssign:
          break;
        case ExpressionType.AddAssignChecked:
          break;
        case ExpressionType.AddChecked:
          break;
        case ExpressionType.And:
          break;
        case ExpressionType.AndAlso:
          break;
        case ExpressionType.AndAssign:
          break;
        case ExpressionType.ArrayIndex:
          break;
        case ExpressionType.ArrayLength:
          break;
        case ExpressionType.Assign:
          break;
        case ExpressionType.Block:
          break;
        case ExpressionType.Call:
          break;
        case ExpressionType.Coalesce:
          break;
        case ExpressionType.Conditional:
          break;
        case ExpressionType.Constant:
          break;
        case ExpressionType.Convert:
          break;
        case ExpressionType.ConvertChecked:
          break;
        case ExpressionType.DebugInfo:
          break;
        case ExpressionType.Decrement:
          break;
        case ExpressionType.Default:
          break;
        case ExpressionType.Divide:
          break;
        case ExpressionType.DivideAssign:
          break;
        case ExpressionType.Dynamic:
          break;
        case ExpressionType.Equal:
          break;
        case ExpressionType.ExclusiveOr:
          break;
        case ExpressionType.ExclusiveOrAssign:
          break;
        case ExpressionType.Extension:
          break;
        case ExpressionType.Goto:
          break;
        case ExpressionType.GreaterThan:
          break;
        case ExpressionType.GreaterThanOrEqual:
          break;
        case ExpressionType.Increment:
          break;
        case ExpressionType.Index:
          break;
        case ExpressionType.Invoke:
          break;
        case ExpressionType.IsFalse:
          break;
        case ExpressionType.IsTrue:
          break;
        case ExpressionType.Label:
          break;
        case ExpressionType.Lambda:
          break;
        case ExpressionType.LeftShift:
          break;
        case ExpressionType.LeftShiftAssign:
          break;
        case ExpressionType.LessThan:
          break;
        case ExpressionType.LessThanOrEqual:
          break;
        case ExpressionType.ListInit:
          break;
        case ExpressionType.Loop:
          break;
        case ExpressionType.MemberAccess:
          break;
        case ExpressionType.MemberInit:
          break;
        case ExpressionType.Modulo:
          break;
        case ExpressionType.ModuloAssign:
          break;
        case ExpressionType.Multiply:
          break;
        case ExpressionType.MultiplyAssign:
          break;
        case ExpressionType.MultiplyAssignChecked:
          break;
        case ExpressionType.MultiplyChecked:
          break;
        case ExpressionType.Negate:
          break;
        case ExpressionType.NegateChecked:
          break;
        case ExpressionType.New:
          break;
        case ExpressionType.NewArrayBounds:
          break;
        case ExpressionType.NewArrayInit:
          break;
        case ExpressionType.Not:
          break;
        case ExpressionType.NotEqual:
          break;
        case ExpressionType.OnesComplement:
          break;
        case ExpressionType.Or:
          break;
        case ExpressionType.OrAssign:
          break;
        case ExpressionType.OrElse:
          break;
        case ExpressionType.Parameter:
          break;
        case ExpressionType.PostDecrementAssign:
          break;
        case ExpressionType.PostIncrementAssign:
          break;
        case ExpressionType.Power:
          break;
        case ExpressionType.PowerAssign:
          break;
        case ExpressionType.PreDecrementAssign:
          break;
        case ExpressionType.PreIncrementAssign:
          break;
        case ExpressionType.Quote:
          break;
        case ExpressionType.RightShift:
          break;
        case ExpressionType.RightShiftAssign:
          break;
        case ExpressionType.RuntimeVariables:
          break;
        case ExpressionType.Subtract:
          break;
        case ExpressionType.SubtractAssign:
          break;
        case ExpressionType.SubtractAssignChecked:
          break;
        case ExpressionType.SubtractChecked:
          break;
        case ExpressionType.Switch:
          break;
        case ExpressionType.Throw:
          break;
        case ExpressionType.Try:
          break;
        case ExpressionType.TypeAs:
          break;
        case ExpressionType.TypeEqual:
          break;
        case ExpressionType.TypeIs:
          break;
        case ExpressionType.UnaryPlus:
          break;
        case ExpressionType.Unbox:
          break;
        default:
          break;
      }
    }
  }
}
