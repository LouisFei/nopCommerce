EF相关的数据访问相关的类封装和扩展。
里面最关键的就是Mapping，Nop采用代码API的形式来建立Model和数据库表之间的映射，命名都是以“表名+Map”的形式，比如：
public partial class BlogCommentMap

