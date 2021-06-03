# DbTool

һ��֧�� DbFirst��ModelFirst �� CodeFirst �����ݿ⹤�ߡ�

[![Build Status](https://weihanli.visualstudio.com/Pipelines/_apis/build/status/WeihanLi.DbTool?branchName=wpf-dev)](https://weihanli.visualstudio.com/Pipelines/_build/latest?definitionId=18&branchName=wpf-dev)

[![GitHub release](https://img.shields.io/github/release/WeihanLi/DbTool.svg?style=plastic)](https://github.com/WeihanLi/DbTool/releases/latest)

## ���

����һ����� `SqlServer` �� `C#` �����ݿ��С���ߣ������������С�����������ݿ���Ӧ�� Model�����һ��ж����ݱ����Ƿ����Ϊ�գ�����Ϊ�յ�����»�ʹ�ÿɿյ��������ͣ���
int? , DateTime? ��������ݿ�������������Ϣ��Ҳ�����������������������������ע�ͣ�֧�ֵ�����������Ե�����Excel�����Ը���Excel�ֶ��ĵ�����Sql�����ݿ����ɾ����û�б��ݵ�ʱ��ͺ��а����ˣ�����֧�ַ���ĸ������ɵ�Modelȥ���ɴ������ݿ���Sql��

> ### ע����ʱ��֧���������������Ҫ��������һ�ű�����ݹ�ϵ

[����DbTool](https://github.com/WeihanLi/DbTool/releases)

## Ϊʲôʹ����

1. [x] ���ݱ��ֶ���Ϣ���ɴ����� Sql��Model First��
1. [x] ���� Excel �ļ����ɴ������ Sql��Model First��
1. [x] �������ݿ����Ϣ�������ݿ�� Excel �ĵ���Db First��
1. [x] �������ݿ����Ϣ���� Model �ļ���֧�������пɿյ���Ϊ�ɿ���������/֧�ֵ�����������Ϣ��Db First��
1. [x] ���� Model ���� sql ��䣨Code First��
1. [x] ֧��һ�ε����������ݱ�/֧��һ��ѡ���� Model �ļ�
1. [x] ֧�� SqlServer��MySql��PostgreSql

## ��չ����

- [ ] �ḻ���(�������ָ�ϣ�<https://github.com/WeihanLi/DbTool/blob/packages/README.md>)

## ����һ��

![DbFirst](resources/desc0.png)

![ModelFirst](resources/desc1.png)

![CodeFirst](resources/desc2.png)

![Settings](resources/desc3.png)

## ʹ��˵��

1. DbFirst

    1. �������ݿ��Model

        1. �������ݿ������ַ��������������ݿ�
        1. ���õ��� Model ѡ��
        1. ѡ��Ҫ���� Model �����ݿ��
        1. ���� Model

    > Model ����ѡ��˵����
    > - model�������ռ䣺���ɵ� model ���ڵ������ռ䣬Ĭ��Ϊ Models������Ϊ�գ�Ϊ����ʹ��Ĭ��ֵModels
    > - modelǰ׺�������Ļ����ϼӵ�ǰ׺��Ĭ��Ϊ��
    > - model��׺�������Ļ����ϼӵĺ�׺��Ĭ��Ϊ��
    > - ����˽���ֶΣ�Ĭ��Ϊ `false`�����Ϊ`false`��ʹ���Զ����ԵĴ����񣬷�֮��ʹ�ô�ͳ `get;set;` �����룬�磺
    >     ``` csharp
    >     public int Id { get; set; } //�Զ����Է�����
    >
    >     private int id1;
    >     public int Id1 { get{ return id1; } set { id1 = value; } } //��ͳget;set������
    >     ```
    > - ���� Description Attribute��Ĭ��ֵΪ `true`�����Ϊ`true`����������ֶ�������һ��`[Description]`��Attribute�����Ϊfalse�����ɣ�Ч�����£�
    >   ``` csharp
    >   /// <summary>�û���</summary>
    >   [Description("�û���")]
    >   public string UserName { get;set; }
    >   ```
    > - ���� Model ���ƵĹ���˵�������ɵ�Model��������ǰ׺�ͱ����Ƽ���׺ƴ�Ӷ��ɣ���������� `tab`��`tab_`��`tbl`��`tbl_`��Щ��ͷ������Ȱ���Щ�Ƴ�����ƴ�ӣ�����ͨ���Զ�������չ

    1. �������ݿ��Excel

        1. �������ݿ������ַ��������������ݿ�
        1. ѡ��Ҫ������ Excel �����ݿ��
        1. ������ Excel

2. ModelFirst

    1. �����ֶ���д��Ϣ������ѡ�� Excel ����
    1. ֻ���� Sql ��䣬�����Զ������ݿ��ﴴ�����������ɵ� Sql ����ٴ�����

3. CodeFirst

    1. �������е� Model ���ɴ������ sql ���ֶ�ע�ͻ�����Ե� `Description` Attribute �л�ȡ
    1. ������Ҫ�����Ƿ��������ݿ����� sql
    1. ������ɱ��ֶ���Ϣ���Ҳ����ɴ������sql�����ɵ� Sql �����ο��������и�����Ҫ�������������Լ��ֶγ���

4. Settings

    1. �޸�Ĭ�����ݿ������ַ���
    1. �޸�Ĭ�����ݿ�����
    1. �޸�Ĭ������

## Contact Me

�����������ʲô���⣬��ӭ��ϵ�� <weihanli@outlook.com>

����������[�������](https://github.com/WeihanLi/DbTool/issues/new)
