// SPDX-License-Identifier: MIT

using System.Numerics;

namespace fennecs.tests;

public class IdentityTests(ITestOutputHelper output)
{
    [Fact]
    public void Virtual_Entities_have_no_Successors()
    {
        Assert.Throws<InvalidCastException>(() => MatchOld.idAny.Successor);
        Assert.Throws<InvalidCastException>(() => MatchOld.idObject.Successor);
        Assert.Throws<InvalidCastException>(() => MatchOld.idTarget.Successor);
        Assert.Throws<InvalidCastException>(() => MatchOld.idEntity.Successor);
        Assert.Throws<InvalidCastException>(() => MatchOld.idPlain.Successor);
    }


    [Fact]
    public void Entity_Resolves_as_Type()
    {
        var entity = new Identity(123);
        Assert.Equal(typeof(Identity), entity.Type);

        var objEntity = Identity.Of("hello world");
        Assert.Equal(typeof(string), objEntity.Type);
    }


    [Fact]
    public void Identity_Plain_is_default()
    {
        var none = MatchOld.idPlain;
        Assert.Equal(default, none.Generation);
        output.WriteLine(none.Generation.ToString());
        output.WriteLine(none.ToString());
        Assert.Equal(default, none);
    }


    [Fact]
    public void Identity_ToString()
    {
        _ = MatchOld.Any.ToString();
        _ = MatchOld.Entity.ToString();
        _ = MatchOld.Target.ToString();
        _ = MatchOld.Object.ToString();
        _ = MatchOld.Plain.ToString();
        _ = Identity.Of("hello world").ToString();
        _ = new Identity(123, 456).ToString();
        _ = new Identity(-1, 2).ToString();

        output.WriteLine(MatchOld.Any.ToString());
        output.WriteLine(MatchOld.Entity.ToString());
        output.WriteLine(MatchOld.Target.ToString());
        output.WriteLine(MatchOld.Object.ToString());
        output.WriteLine(MatchOld.Plain.ToString());
        output.WriteLine(Identity.Of("hello world").ToString());
        output.WriteLine(new Identity(123, 456).ToString());
        output.WriteLine(new Identity(-1, 2).ToString());
    }


    [Fact]
    public void Identity_None_cannot_Match_One()
    {
        var zero = new Identity(0);
        Assert.NotEqual(MatchOld.Plain, new(zero));

        var one = new Identity(1);
        Assert.NotEqual(MatchOld.Plain, new(one));
    }


    [Fact]
    public void Identity_Matches_Only_Self()
    {
        var self = new Identity(12345);
        Assert.Equal(self, self);

        var successor = new Identity(12345, 3);
        Assert.NotEqual(self, successor);

        var other = new Identity(9000, 3);
        Assert.NotEqual(self, other);
    }


    [Theory]
    [InlineData(1500, 1500)]
    public void Identity_HashCodes_are_Unique(TypeID idCount, TypeID genCount)
    {
        var ids = new Dictionary<int, Identity>((int) (idCount * genCount * 4f));

        //Identities
        for (var i = 0; i < idCount; i++)
            //Generations
        for (TypeID g = 1; g < genCount; g++)
        {
            var identity = new Identity(i, g);

            Assert.NotEqual(new(identity), MatchOld.Any);
            Assert.NotEqual(new(identity), MatchOld.Plain);

            if (ids.ContainsKey(identity.GetHashCode()))
                Assert.Fail($"Collision of {identity} with {ids[identity.GetHashCode()]}, {identity.GetHashCode()}#==#{ids[identity.GetHashCode()].GetHashCode()}");
            else
                ids.Add(identity.GetHashCode(), identity);
        }
    }


    [Fact]
    public void Equals_Prevents_Boxing_as_InvalidCastException()
    {
        object o = "don't @ me";
        var id = new Identity(69, 420);
        Assert.Throws<InvalidCastException>(() => id.Equals(o));
    }


    [Fact]
    public void Any_and_None_are_Distinct()
    {
        Assert.NotEqual(MatchOld.Any, MatchOld.Plain);
        Assert.NotEqual(MatchOld.Any.GetHashCode(), MatchOld.Plain.GetHashCode());
    }


    [Fact]
    public void Identity_Matches_Self_if_Same()
    {
        var random = new Random(420960);
        for (var i = 0; i < 1_000; i++)
        {
            var id = random.Next();
            var gen = (TypeID) random.Next();

            var self = new Identity(id, gen);
            var other = new Identity(id, gen);

            Assert.Equal(self, other);
        }
    }


    /*
    [Fact]
    private void Entity_ToString_Facades_Identity_ToString()
    {
        var identity = new Identity(123, 456);
        var identity = new Identity(identity);
        output.WriteLine(identity.ToString());
        Assert.Equal(identity.ToString(), identity.ToString());
    }
    */


    [Fact]
    public void Entity_HashCode_is_Stable()
    {
        using var world = new World();
        var entity1 = world.Spawn();
        var entity2 = world.Spawn();
        var hash1 = entity1.GetHashCode();
        var hash2 = entity2.GetHashCode();
        Assert.NotEqual(hash1, hash2);
        Assert.Equal(hash1, entity1.GetHashCode());
        Assert.Equal(hash2, entity2.GetHashCode());
    }


    [Fact]
    private void Identity_is_Equal_to_Itself()
    {
        using var world = new World();
        var identity = world.Spawn();
        Assert.Equal(identity, identity);
    }


    [Fact]
    private void Same_Entity_is_Equal()
    {
        var entity1 = new Identity(123, 999);
        var entity2 = new Identity(123, 999);
        Assert.Equal(entity1, entity2);
        Assert.True(entity1 == entity2);
    }


    [Fact]
    private void Different_Entity_is_Not_Equal()
    {
        var entity1 = new Identity(69, 420);
        var entity2 = new Identity(420, 69);

        var entity3 = new Identity(69, 69);
        var entity4 = new Identity(420, 420);

        Assert.NotEqual(entity1, entity2);
        Assert.True(entity1 != entity2);

        Assert.NotEqual(entity3, entity4);
        Assert.True(entity3 != entity4);

        Assert.NotEqual(entity1, entity3);
        Assert.True(entity1 != entity3);

        Assert.NotEqual(entity2, entity4);
        Assert.True(entity2 != entity4);
    }


    [Fact]
    public void Entity_is_Alive_after_Spawn()
    {
        using var world = new World();
        var identity = world.Spawn();
        Assert.True(world.IsAlive(identity));
    }


    [Fact]
    private void Entity_is_Not_Alive_after_Despawn()
    {
        using var world = new World();
        var identity = world.Spawn();
        world.Despawn(identity);
        Assert.False(world.IsAlive(identity));
    }


    [Fact]
    private void Entity_has_no_Components_after_Spawn()
    {
        using var world = new World();
        var identity = world.Spawn();
        var components = world.GetSignature(identity);
        Assert.False(world.HasComponent<int>(identity, default));
        Assert.True(components.Count() == 1);
    }


    [Theory]
    [ClassData(typeof(ComponentDataSource))]
    private void Entity_can_Add_Component<T>(T t1) where T : struct
    {
        using var world = new World();
        var identity = world.Spawn();
        identity.Add(t1);
        Assert.True(world.HasComponent<T>(identity, default));
        var components = world.GetSignature(identity);
        Assert.True(components.Count() == 2);
    }


    [Theory]
    [ClassData(typeof(ComponentDataSource))]
    private void Entity_cannot_Get_Component_from_Dead<T>(T t1) where T : struct
    {
        using var world = new World();
        var entity = world.Spawn().Add(t1);
        world.Despawn(entity);

        Assert.Throws<ObjectDisposedException>(() => world.GetComponent<T>(entity, default));
    }


    [Theory]
    [ClassData(typeof(ComponentDataSource))]
    private void Entity_cannot_Get_Component_from_Successor<T>(T t1) where T : struct
    {
        using var world = new World(0);
        var entity1 = world.Spawn().Add(t1);
        world.Despawn(entity1);
        var entity2 = world.Spawn().Add(t1);

        Assert.Equal(entity1.Id.Index, entity2.Id.Index);
        Assert.Throws<ObjectDisposedException>(() => world.GetComponent<T>(entity1, default));
    }


    [Theory]
    [ClassData(typeof(ComponentDataSource))]
    private void Entity_can_Get_Component<T>(T t1) where T : struct
    {
        using var world = new World();
        var identity = world.Spawn().Add(t1).Id;
        var x = world.GetComponent<T>(identity);
        Assert.Equal(t1, x);
    }


    [Theory]
    [ClassData(typeof(ComponentDataSource))]
    private void Entity_can_Remove_Component<T>(T t1) where T : struct
    {
        using var world = new World();
        var identity = world.Spawn();
        identity.Add(t1);
        identity.Remove<T>();
        Assert.False(world.HasComponent<T>(identity, default));
    }


    [Theory]
    [ClassData(typeof(ComponentDataSource))]
    private void Entity_can_ReAdd_Component<T>(T t1) where T : struct
    {
        using var world = new World();
        var identity = world.Spawn();
        identity.Add(t1);
        identity.Remove<T>();
        identity.Add(t1);
        Assert.True(world.HasComponent<T>(identity, default));
    }


    [Theory]
    [ClassData(typeof(ComponentDataSource))]
    private void Entity_cannot_Add_Component_twice<T>(T t1) where T : struct
    {
        using var world = new World();
        var identity = world.Spawn();
        identity.Add(t1);
        Assert.Throws<ArgumentException>(() => identity.Add(t1));
    }


    [Theory]
    [ClassData(typeof(ComponentDataSource))]
    private void Entity_cannot_Remove_Component_twice<T>(T t1) where T : struct
    {
        using var world = new World();
        var identity = world.Spawn();
        identity.Add(t1);
        identity.Remove<T>();
        Assert.Throws<ArgumentException>(() => identity.Remove<T>());
    }


    [Theory]
    [ClassData(typeof(ComponentDataSource))]
#pragma warning disable xUnit1026
    private void Entity_cannot_Remove_Component_without_Adding<T>(T _) where T : struct
    {
        using var world = new World();
        var identity = world.Spawn();
        Assert.Throws<ArgumentException>(() => identity.Remove<T>());
    }
#pragma warning restore xUnit1026


    #region Input Data

    private struct CompoundComponent
    {
        // ReSharper disable once NotAccessedField.Local
        public required bool B1;

        // ReSharper disable once NotAccessedField.Local
        public required int I1;
    }


    private class ComponentDataSource : List<object[]>
    {
        public ComponentDataSource()
        {
            Add([123]);
            Add([1.23f]);
            Add([float.NegativeInfinity]);
            Add([float.NaN]);
            Add([new Vector2(1, 2)]);
            Add([new Vector3(1, 2, 3)]);
            Add([new Vector4(1, 2, 3, 4)]);
            Add([new Matrix4x4()]);
            Add([new CompoundComponent {B1 = true, I1 = 5}]);
            Add([new CompoundComponent {B1 = default, I1 = default}]);
        }
    }

    #endregion
}