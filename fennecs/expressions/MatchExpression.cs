﻿namespace fennecs.expressions;

internal readonly record struct Match(TypeExpression value)
{
    public static Match Any<T>() => new(TypeExpression.Of<T>(MatchOld.Any));
    public static Match AnyEntity<T>() => new(TypeExpression.Of<T>(MatchOld.Entity));
    public static Match AnyTarget<T>() => new(TypeExpression.Of<T>(MatchOld.Target));
    public static Match AnyObject<T>() => new(TypeExpression.Of<T>(MatchOld.Object));
    public static Match OnlyPlain<T>() => new(TypeExpression.Of<T>(MatchOld.Plain));

    public static Match Plain<T>() => new(TypeExpression.Of<T>(MatchOld.Plain));
    public static Match Entity<T>(Entity target) => new(TypeExpression.Of<T>(target));
    public static Match Object<T>(T target) where T : class => new(TypeExpression.Of<T>(Link.With(target)));
    
    internal bool Matches(TypeExpression other) => value.Matches(other);
    internal bool Matches(Match other) => value.Matches(other.value);
    internal bool Matches(Component other) => value.Matches(other.value);
}


internal readonly record struct Component(TypeExpression value)
{
    public static Component Plain<T>() => new(TypeExpression.Of<T>(MatchOld.Plain));
    public static Component Entity<T>(Entity target) => new(TypeExpression.Of<T>(target));
    public static Component Object<T>(T target) where T : class => new(TypeExpression.Of<T>(Link.With(target)));
    
    internal bool Matches(TypeExpression other) => value.Matches(other);
    internal bool Matches(Match other) => value.Matches(other.value);
    internal bool Matches(Component other) => value.Matches(other.value);
}
