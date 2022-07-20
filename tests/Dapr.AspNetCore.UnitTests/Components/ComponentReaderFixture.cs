using CodeArchitects.Platform.Dapr.AspNetCore.Components.Schema;
using Microsoft.Extensions.FileProviders;
using Moq;
using System.Reflection;
using System.Text;
using Xunit.Sdk;

namespace CodeArchitects.Platform.Dapr.AspNetCore.Components;

public static class ComponentReaderFixture
{
  public class ComponentDataAttribute : DataAttribute
  {
    private static IEnumerable<string> ComponentTexts
    {
      get
      {
        yield return @"
apiVersion: dapr.io/v1alpha1
kind: Component
metadata:
  name: storename
  namespace: storenamespace
spec:
  type: state.redis
  metadata:
  - name: redisHost
    value: redis:6379
  - name: redisPassword
    value: """"
  - name: actorStateStore
    value: ""true""
";
        yield return @"
apiVersion: dapr.io/v1alpha1
kind: Component
metadata:
  name: busname
  namespace: busnamespace
spec:
  type: pubsub.redis
  version: v1
  metadata:
  - name: redisHost
    value: redis:6379
  - name: redisPassword
    value: KeFg23
";
      }
    }

    private static IEnumerable<ComponentSchema> Components
    {
      get
      {
        yield return new ComponentSchema
        {
          ApiVersion = "dapr.io/v1alpha1",
          Kind = "Component",
          Metadata = new MetadataSchema
          {
            Name = "storename",
            Namespace = "storenamespace"
          },
          Spec = new SpecSchema
          {
            Type = "state.redis",
            Metadata = new MetadataItemSchema[]
            {
              new MetadataItemSchema
              {
                Name = "redisHost",
                Value = "redis:6379"
              },
              new MetadataItemSchema
              {
                Name = "redisPassword",
                Value = ""
              },
              new MetadataItemSchema
              {
                Name = "actorStateStore",
                Value = "true"
              }
            }
          }
        };
        yield return new ComponentSchema
        {
          ApiVersion = "dapr.io/v1alpha1",
          Kind = "Component",
          Metadata = new MetadataSchema
          {
            Name = "busname",
            Namespace = "busnamespace"
          },
          Spec = new SpecSchema
          {
            Type = "pubsub.redis",
            Version = "v1",
            Metadata = new MetadataItemSchema[]
            {
              new MetadataItemSchema
              {
                Name = "redisHost",
                Value = "redis:6379"
              },
              new MetadataItemSchema
              {
                Name = "redisPassword",
                Value = "KeFg23"
              }
            }
          }
        };
      }
    }

    public override IEnumerable<object[]> GetData(MethodInfo testMethod)
    {
      return ComponentTexts.Zip(Components).Select(item =>
      {
        Stream stream = new MemoryStream(Encoding.UTF8.GetBytes(item.First));
        Mock<IFileInfo> fileMock = new Mock<IFileInfo>(MockBehavior.Strict);
        fileMock
          .Setup(x => x.CreateReadStream())
          .Returns(stream);
        return new object[] { fileMock.Object, item.Second };
      });
    }
  }
}
