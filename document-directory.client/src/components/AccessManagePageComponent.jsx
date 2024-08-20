import React, {useEffect, useState} from 'react';
import TableWithNodesComponent from './TableWithNodesComponent';


function AccessManagePageComponent() {

    async function getAvailableNodes() {
        const response = await fetch("https://localhost:7018/api/NodeHierarchy", {
            credentials: 'include',
            headers: new Headers({'Access-Control-Allow-Credentials': 'true'})
        });
        if (response.status == 200)
        {
            const json = await response.json();
            json.map((node) => {
                if (node.type == "Document")
                {
                    const activityEnd = dayjs(node.activityEnd, 'YYYY-MM-DD').format('DD-MM-YYYY');
                    const createdAt = dayjs(node.createdAt, 'YYYY-MM-DD').format('DD-MM-YYYY');
                    setAvailableNodes(prevNodes => [...prevNodes, {...node, activityEnd: activityEnd, createdAt: createdAt}]);
                }
                else {
                    const createdAt = dayjs(node.createdAt, 'YYYY-MM-DD').format('DD-MM-YYYY');
                    setAvailableNodes(prevNodes => [...prevNodes, {...node, createdAt: createdAt}]);
                }
            });
        }
    }

    useEffect(() => {
        getAvailableNodes();
    }, [])

    return (
        <div>
            <TableWithNodesComponent availableNodes={availableNodes}/>
        </div>
    )

}


export default AccessManagePageComponent;
