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
        protected string _modelpath;  //ʵ����������ռ�
        protected List<ColumnModel> _fieldlist;  //ѡ����ֶμ���
        protected string _modelName;  //ʵ������
        #endregion

        /// <summary>
        /// ���캯��
        /// </summary>
        /// <param name="fieldList">ѡ����ֶμ���</param>
        /// <param name="modelPath">ʵ����������ռ�</param>
        /// <param name="modelPrefix">ʵ�����ǰ׺</param>
        /// <param name="modelSuffix">ʵ����ĺ�׺</param>
        public BuilderModel(List<ColumnModel> fieldList, string modelPath, string modelPrefix, string modelSuffix)
        {
            
            _fieldlist = fieldList;
            _modelpath = modelPath;
            string tableName = _fieldlist[0].TableName;
            _modelName = modelPrefix + tableName.ToPascalCase() + modelSuffix;
        }


        /// <summary>
        /// ��������Model��
        /// </summary>
        public abstract string CreatModel();





    }
}
