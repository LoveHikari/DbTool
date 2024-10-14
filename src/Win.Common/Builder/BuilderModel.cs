using Hikari.Common;
using System.Collections.Generic;
using System.Text;
using Win.Models;

namespace Win.Common.Builder
{
    /// <summary>
    /// ��ϵ�л���Model�����������
    /// </summary>
    public abstract class BuilderModel
    {
        #region �ֶ�
        protected List<ColumnModel> _fieldList;  //ѡ����ֶμ���
        protected string _modelPath;  //ʵ����������ռ�
        protected string _modelName;  //ʵ������
        protected string _repositoryPath;  //�ִ���������ռ�
        protected string _repositoryName;  //�ִ�����
        protected string _applicationPath;  //ҵ����������ռ�
        protected string _applicationName;  //ҵ������
        #endregion

        /// <summary>
        /// ���캯��
        /// </summary>
        /// <param name="fieldList">ѡ����ֶμ���</param>
        /// <param name="modelPath">ʵ����������ռ�</param>
        /// <param name="modelPrefix">ʵ�����ǰ׺</param>
        /// <param name="modelSuffix">ʵ����ĺ�׺</param>
        /// <param name="repositoryPath">�ִ���������ռ�</param>
        /// <param name="repositoryPrefix">�ִ����ǰ׺</param>
        /// <param name="repositorySuffix">�ִ���ĺ�׺</param>
        /// <param name="applicationPath">ҵ����������ռ�</param>
        /// <param name="applicationPrefix">ҵ�����ǰ׺</param>
        /// <param name="applicationSuffix">ҵ����ĺ�׺</param>
        protected BuilderModel(List<ColumnModel> fieldList, string modelPath, string modelPrefix, string modelSuffix, 
            string repositoryPath, string repositoryPrefix, string repositorySuffix,
            string applicationPath, string applicationPrefix, string applicationSuffix)
        {
            
            _fieldList = fieldList;
            _modelPath = modelPath;
            string tableName = _fieldList[0].TableName;
            _modelName = modelPrefix + tableName.ToPascalCase() + modelSuffix;

            _repositoryPath = repositoryPath;
            _repositoryName = repositoryPrefix + tableName.ToPascalCase() + repositorySuffix;

            _applicationPath = applicationPath;
            _applicationName = applicationPrefix + tableName.ToPascalCase() + applicationSuffix;
        }


        /// <summary>
        /// ��������Model��
        /// </summary>
        public abstract string CreatModel();
        /// <summary>
        /// ���ɲִ��ӿ�
        /// </summary>
        /// <returns></returns>
        public abstract string CreatRepositoryInterface();

        /// <summary>
        /// ���ɲִ���
        /// </summary>
        /// <returns></returns>
        public abstract string CreatRepository();

        /// <summary>
        /// ����ҵ��ӿ�
        /// </summary>
        /// <returns></returns>
        public abstract string CreatApplicationInterface();

        /// <summary>
        /// ����ҵ����
        /// </summary>
        /// <returns></returns>
        public abstract string CreatApplication();

    }
}
