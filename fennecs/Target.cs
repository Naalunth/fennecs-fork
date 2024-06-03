﻿namespace fennecs;

/// <summary>
/// Match Expressions
/// </summary>
/// <para>
/// Match's static readonly constants differentiate between Plain Components, Entity-Entity Relations, and Entity-Object Relations.
/// The class offers a set of Wildcards for matching combinations of the above in <see cref="Query">Queries</see>; as opposed to filtering for only a specific target.
/// </para>
public readonly record struct Target 
{
    private Identity Value { get; }
    
    internal Target(Identity Value) => this.Value = Value;

    /// <summary>
    /// <para>
    /// Match Expression to match only a specific Relation (Entity-Entity).
    /// </para>
    /// <para>Use it freely in filter expressions. See <see cref="QueryBuilder"/> for how to apply it in queries.</para>
    /// </summary>
    public static Target Relation(Entity other) => new(other.Id);
    
    /// <summary>
    /// <para>
    /// Match Expression to match only a specific Object Link (Entity-Object).
    /// </para>
    /// <para>Use it freely in filter expressions. See <see cref="QueryBuilder"/> for how to apply it in queries.</para>
    /// </summary>
    public static Target Link<T>(T link) where T : class => new(Identity.Of<T>(link));

    /// <summary>
    /// <para><b>Wildcard match expression for Entity iteration.</b><br/>This matches all types of relations on the given Stream Type: <b>Plain, Entity, and Object</b>.
    /// </para>
    /// <para>This expression is free when applied to a Filter expression, see <see cref="Query"/>.
    /// </para>
    /// <para>Applying this to a Query's Stream Type can result in multiple iterations over entities if they match multiple component types. This is due to the wildcard's nature of matching all components.</para>
    /// </summary>
    /// <remarks>
    /// <para>⚠️ Using wildcards can lead to a CROSS JOIN effect, iterating over entities multiple times for
    /// each matching component. While querying is efficient, this increases the number of operations per entity.</para>
    /// <para>This is an intentional feature, and <c>Match.Any</c> is the default as usually the same backing types are not re-used across
    /// relations or links; but if they are, the user likely wants their Query to enumerate all of them.</para>
    /// <para>This effect is more pronounced in large archetypes with many matching components, potentially
    /// multiplying the workload significantly. However, for smaller archetypes or simpler tasks, impacts are minimal.</para>
    /// <para>Risks and considerations include:</para>
    /// <ul>
    /// <li>Repeated enumeration: Entities matching a wildcard are processed multiple times, for each matching
    /// component type combination.</li>
    /// <li>Complex queries: Especially in Archetypes where Entities match multiple components, multiple wildcards
    /// can create a cartesian product effect, significantly increasing complexity and workload.</li>
    /// <li>Use wildcards deliberately and sparingly.</li>
    /// </ul>
    /// </remarks>
    public static Target Any => new(idAny); // or prefer default ?

    /// <summary>
    /// <b>Wildcard match expression for Entity iteration.</b><br/>Matches any non-plain Components of the given Stream Type, i.e. any with a <see cref="TypeExpression.Target"/>.
    /// <para>This expression is free when applied to a Filter expression, see <see cref="Query"/>.
    /// </para>
    /// <para>Applying this to a Query's Stream Type can result in multiple iterations over entities if they match multiple component types. This is due to the wildcard's nature of matching all components.</para>
    /// </summary>
    /// <inheritdoc cref="Any"/>
    public static Target AnyTarget => new(idTarget);
    
    /// <summary>
    /// <para>Wildcard match expression for Entity iteration. <br/>This matches all <b>Entity-Object</b> Links of the given Stream Type.
    /// </para>
    /// <para>Use it freely in filter expressions to match any component type. See <see cref="QueryBuilder"/> for how to apply it in queries.</para>
    /// <para>This expression is free when applied to a Filter expression, see <see cref="Query"/>.
    /// </para>
    /// <para>Applying this to a Query's Stream Type can result in multiple iterations over entities if they match multiple component types. This is due to the wildcard's nature of matching all components.</para>
    /// </summary>
    /// <inheritdoc cref="Any"/>
    public static Target Object => new(idObject);

    /// <summary>
    /// <para><b>Wildcard match expression for Entity iteration.</b><br/>This matches only <b>Entity-Entity</b> Relations of the given Stream Type.
    /// </para>
    /// <para>This expression is free when applied to a Filter expression, see <see cref="Query"/>.
    /// </para>
    /// <para>Applying this to a Query's Stream Type can result in multiple iterations over entities if they match multiple component types. This is due to the wildcard's nature of matching all components.</para>
    /// </summary>
    /// <inheritdoc cref="Any"/>
    [Obsolete("Use Entity.Any")]
    public static Target Entity => new(idEntity);


    internal bool Matches(Target other) => Value == other.Value;
    
    /// <summary>
    /// <para>Implicitly convert an <see cref="Identity"/> to a <see cref="Target"/> for use in filter expressions.</para>
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    //public static implicit operator Match(Identity value) => new(value);
    public static implicit operator Target(Entity value) => new(value);

    //public static implicit operator Match(Identity value) => new(value);
    
    //TODO Maybe not even needed...
    internal bool IsWildcard => Value.IsWildcard;
    internal bool IsEntity => Value.IsEntity;
    internal bool IsObject => Value.IsObject;
    internal bool IsTarget => Value == idTarget;
    internal bool IsPlain => Value == idPlain;
    
    [Obsolete("Maybe find a way to remove me.")]
    internal ulong Raw => Value.Value;
    
    internal void Deconstruct(out Identity identity)
    {
        identity = Value;
    }


    // TODO: decide encoding whether new(-1/*int.MinValue*/, 0); etc...
    internal static readonly Identity idPlain = default;
    internal static readonly Identity idEntity = new(-3, 0);
    internal static readonly Identity idObject = new(-4, 0);
    internal static readonly Identity idAny = new(-1, 0);
    internal static readonly Identity idTarget = new(-2, 0);
}
