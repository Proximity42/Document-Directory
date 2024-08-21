import {Table, Form} from 'antd';


function TableWithNodesComponent({availableNodes, columns}) {
    return (
        <Table 
            columns={columns}
            rowKey={(record) => record.id}
            dataSource={availableNodes}
        />
    )
}

export default TableWithNodesComponent;